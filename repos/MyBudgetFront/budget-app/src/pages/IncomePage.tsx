import React, { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { Link } from 'react-router-dom';
import apiClient from '../api/axios';
import { Income, CreateIncomeRequest } from '../types';
import '../styles/dashboard.css';
import '../styles/income.css';

const IncomePage = () => {
  const [incomes, setIncomes] = useState<Income[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [editingId, setEditingId] = useState<string | null>(null);
  const currency = localStorage.getItem('userCurrency') || "PLN";

  const { register, handleSubmit, reset, setValue, formState: { errors } } = useForm<CreateIncomeRequest>({
    defaultValues: {
      date: new Date().toISOString().split('T')[0]
    }
  });

  const fetchIncomes = async () => {
    try {
      const response = await apiClient.get<Income[]>('/api/Income');
      const sorted = response.data.sort((a, b) => 
        new Date(b.date).getTime() - new Date(a.date).getTime()
      );
      setIncomes(sorted);
    } catch (error) {
      console.error("Błąd pobierania przychodów:", error);
    }
  };

  useEffect(() => {
    fetchIncomes();
  }, []);

  const onSubmit = async (data: CreateIncomeRequest) => {
    setIsLoading(true);
    try {
      const payload = {
        amount: Number(data.amount),
        date: new Date(data.date).toISOString(),
        source: data.source
      };

      if (editingId) {
        await apiClient.put(`/api/Income/${editingId}`, payload);
      } else {
        await apiClient.post('/api/Income', payload);
      }
      
      resetForm();
      fetchIncomes();
    } catch (error) {
      console.error("Błąd zapisu:", error);
      alert("Wystąpił błąd podczas zapisu.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Czy na pewno chcesz usunąć ten przychód?")) return;
    try {
      await apiClient.delete(`/api/Income/${id}`);
      setIncomes(prev => prev.filter(i => i.id !== id));
    } catch (error) {
      console.error("Błąd usuwania:", error);
      alert("Nie udało się usunąć wpisu.");
    }
  };

  const handleEdit = (income: Income) => {
    setEditingId(income.id);
    setValue("source", income.source || "");
    setValue("amount", income.amount);
    setValue("date", income.date.split('T')[0]);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const resetForm = () => {
    setEditingId(null);
    reset({
      source: "",
      amount: 0,
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
          <Link to="/expenses">Wydatki</Link>
          <Link to="/income" className="sidebar-link-active">Przychody</Link>
          <Link to="/settings">Ustawienia</Link>
        </nav>
        <div style={{ marginTop: 'auto' }}>
          <button onClick={handleLogout}>Wyloguj się</button>
        </div>
      </aside>

      <main className="main-content">
        <h1>Twoje Przychody</h1>

        <div className={editingId ? "income-form-card editing-mode" : "income-form-card"}>
          <div className="form-header">
            <h3>{editingId ? 'Edytuj przychód' : 'Dodaj nowy przychód'}</h3>
            {editingId && <button onClick={resetForm} className="cancel-btn">Anuluj</button>}
          </div>

          <form onSubmit={handleSubmit(onSubmit)} className="expense-form">
            
            <div className="form-group-inline form-group-grow">
              <label>Nazwa (Źródło)</label>
              <input 
                type="text" 
                placeholder="np. Wypłata, Sprzedaż..."
                {...register("source", { required: true })} 
              />
            </div>

            <div className="form-group-inline">
              <label>Kwota ({currency})</label>
              <input 
                type="number" 
                step="0.01" 
                {...register("amount", { required: true, min: 0.01 })} 
              />
            </div>

            <div className="form-group-inline">
              <label>Data</label>
              <input 
                type="date" 
                {...register("date", { required: true })} 
              />
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
                <th>Nazwa</th>
                <th className="text-right">Kwota</th>
                <th className="text-center">Akcje</th>
              </tr>
            </thead>
            <tbody>
              {incomes.map((inc) => (
                <tr key={inc.id} className={editingId === inc.id ? "editing-row" : ""}>
                  <td>{new Date(inc.date).toLocaleDateString()}</td>
                  <td>{inc.source}</td>
                  <td className="amount-cell-income">
                    +{inc.amount.toFixed(2)} {currency}
                  </td>
                  <td className="text-center">
                    <button onClick={() => handleEdit(inc)} className="action-btn edit-btn">✏️</button>
                    <button onClick={() => handleDelete(inc.id)} className="action-btn delete-btn">🗑️</button>
                  </td>
                </tr>
              ))}
              {incomes.length === 0 && (
                <tr>
                  <td colSpan={4} className="empty-message">Brak przychodów</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </main>
    </div>
  );
};

export default IncomePage;