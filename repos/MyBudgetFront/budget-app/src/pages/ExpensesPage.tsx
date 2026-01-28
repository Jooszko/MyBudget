import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link } from 'react-router-dom';
import apiClient from '../api/axios';
import { Expense, CreateExpenseRequest, Category } from '../types';
import '../styles/dashboard.css';
import '../styles/expenses.css';

const ExpensesPage = () => {
  const [expenses, setExpenses] = useState<Expense[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);

  const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm<CreateExpenseRequest>({
    defaultValues: {
      date: new Date().toISOString().split('T')[0]
    }
  });

  const fetchData = async () => {
    try {
      const [expensesRes, categoriesRes] = await Promise.all([
        apiClient.get<Expense[]>('/api/Expenses'),
        apiClient.get<Category[]>('/api/Category')
      ]);

      const sortedExpenses = expensesRes.data.sort((a, b) => 
        new Date(b.date).getTime() - new Date(a.date).getTime()
      );
      setExpenses(sortedExpenses);

      setCategories(categoriesRes.data.sort((a, b) => a.name.localeCompare(b.name)));

    } catch (error) {
      console.error("Błąd pobierania danych:", error);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const onSubmit = async (data: CreateExpenseRequest) => {
    setIsLoading(true);
    try {
      const selectedCategory = categories.find(c => c.id === data.categoryName);
      const catName = selectedCategory ? selectedCategory.name : "";

      const payload = {
        amount: Number(data.amount),
        date: new Date(data.date).toISOString(),
        note: data.note,
        categoryName: catName,
        categoryId: data.categoryName 
      };

      if (editingId) {
        await apiClient.put('/api/Expenses', { ...payload, id: editingId });
      } else {
        await apiClient.post('/api/Expenses', payload);
      }
      
      resetForm();
      
      const res = await apiClient.get<Expense[]>('/api/Expenses');
      const sorted = res.data.sort((a,b) => new Date(b.date).getTime() - new Date(a.date).getTime());
      setExpenses(sorted);

    } catch (error: any) {
      console.error("Błąd zapisu:", error);
      if (error.response) {
        alert(`Błąd serwera: ${JSON.stringify(error.response.data)}`);
      } else {
        alert("Wystąpił błąd: " + error.message);
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Usunąć ten wydatek?")) return;
    try {
      await apiClient.delete(`/api/Expenses/${id}`);
      setExpenses(prev => prev.filter(e => e.id !== id));
    } catch (error) {
      console.error("Błąd usuwania:", error);
    }
  };

  const handleEdit = (expense: Expense) => {
    setEditingId(expense.id);
    setValue("categoryName", expense.categoryId); 
    setValue("amount", expense.amount);
    setValue("note", expense.note || "");
    setValue("date", expense.date.split('T')[0]);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const resetForm = () => {
    setEditingId(null);
    reset({
      amount: 0,
      note: "",
      date: new Date().toISOString().split('T')[0]
    });
  };

  const handleLogout = () => {
    localStorage.removeItem('user');
    window.location.reload();
  };

  return (
    <div className="dashboard-container">
      <aside className="sidebar">
        <h2>MyBudget</h2>
        <nav>
          <Link to="/">Pulpit</Link>
          <Link to="/expenses" className="sidebar-link-active">Wydatki</Link>
          <Link to="/income">Przychody</Link>
          <Link to="/settings">Ustawienia</Link>
        </nav>
        <div style={{ marginTop: 'auto' }}>
          <button onClick={handleLogout}>Wyloguj się</button>
        </div>
      </aside>

      <main className="main-content">
        <h1>Twoje Wydatki</h1>

        <div className={editingId ? "expense-form-card editing-mode" : "expense-form-card"}>
          <div className="form-header">
             <h3>{editingId ? 'Edytuj wydatek' : 'Dodaj nowy wydatek'}</h3>
             {editingId && <button onClick={resetForm} className="cancel-btn">Anuluj</button>}
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="expense-form">
            <div className="form-group-inline">
              <label>Kategoria</label>
              <select {...register("categoryName", { required: true })}>
                <option value="" disabled>Wybierz...</option>
                {categories.map(cat => (
                  <option key={cat.id} value={cat.id}>
                    {cat.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group-inline">
              <label>Kwota</label>
              <input type="number" step="0.01" {...register("amount", { required: true })} />
            </div>

            <div className="form-group-inline">
              <label>Data</label>
              <input type="date" {...register("date", { required: true })} />
            </div>

            <div className="form-group-inline form-group-grow">
              <label>Notatka</label>
              <input type="text" {...register("note")} />
            </div>

            <button type="submit" disabled={isLoading} className={editingId ? "save-btn" : "add-btn"}>
              {editingId ? 'Zapisz' : 'Dodaj'}
            </button>
          </form>
        </div>

        <div className="table-card">
          <table className="custom-table">
            <thead>
              <tr>
                <th>Data</th>
                <th>Kategoria</th>
                <th>Opis</th>
                <th className="text-right">Kwota</th>
                <th className="text-center">Akcje</th>
              </tr>
            </thead>
            <tbody>
              {expenses.map((exp) => {
                const matchedCategory = categories.find(c => c.id === exp.categoryId);
                const currency = localStorage.getItem('userCurrency') || "PLN";
                
                return (
                  <tr key={exp.id} className={editingId === exp.id ? "editing-row" : ""}>
                    <td>{new Date(exp.date).toLocaleDateString()}</td>
                    <td>
                      <span className="badge">
                        {matchedCategory ? matchedCategory.name : 'Inne'}
                      </span>
                    </td>
                    <td>{exp.note}</td>
                    <td className="amount-cell-expense">
                      -{exp.amount.toFixed(2)} {currency}
                    </td>
                    <td className="text-center">
                      <button onClick={() => handleEdit(exp)} className="action-btn edit-btn">✏️</button>
                      <button onClick={() => handleDelete(exp.id)} className="action-btn delete-btn">🗑️</button>
                    </td>
                  </tr>
                );
              })}
              {expenses.length === 0 && (
                <tr>
                  <td colSpan={5} className="empty-message">Brak wydatków</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </main>
    </div>
  );
};

export default ExpensesPage;