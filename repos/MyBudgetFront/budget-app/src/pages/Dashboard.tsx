import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { PieChart, Pie, Cell, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import apiClient from '../api/axios';
import { Expense, Income, Category, User } from '../types';
import '../styles/dashboard.css';

const COLORS = ['#3498db', '#2ecc71', '#9b59b6', '#f1c40f', '#e67e22', '#e74c3c', '#95a5a6'];

const Dashboard = () => {
  const navigate = useNavigate();
  
  const getFirstDayOfMonth = () => {
    const date = new Date();
    return new Date(date.getFullYear(), date.getMonth(), 1).toISOString().split('T')[0];
  };

  const getToday = () => {
    return new Date().toISOString().split('T')[0];
  };

  const [balance, setBalance] = useState<number>(0);
  const [allExpenses, setAllExpenses] = useState<Expense[]>([]);
  const [allIncome, setAllIncome] = useState<Income[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [displayedTransactions, setDisplayedTransactions] = useState<(Expense | Income)[]>([]);
  const [chartData, setChartData] = useState<{ name: string; value: number }[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [startDate, setStartDate] = useState(getFirstDayOfMonth());
  const [endDate, setEndDate] = useState(getToday());
  const [currency, setCurrency] = useState<string>("PLN");

  const handleLogout = () => {
    localStorage.removeItem('user');
    window.location.reload();
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        setIsLoading(true);
        
        const [expensesRes, incomeRes, categoriesRes, accountRes] = await Promise.all([
          apiClient.get<Expense[]>('/api/Expenses'),
          apiClient.get<Income[]>('/api/Income'),
          apiClient.get<Category[]>('/api/Category'),
          apiClient.get<User>('/api/Account') 
        ]);

        const userCurrency = accountRes.data.currency || "PLN";
        setCurrency(userCurrency);
        localStorage.setItem('userCurrency', userCurrency);

        setAllExpenses(expensesRes.data);
        setAllIncome(incomeRes.data);
        setCategories(categoriesRes.data);

        const totalIncome = incomeRes.data.reduce((sum, item) => sum + item.amount, 0);
        const totalExpenses = expensesRes.data.reduce((sum, item) => sum + item.amount, 0);
        setBalance(totalIncome - totalExpenses);

      } catch (error) {
        console.error("Błąd danych:", error);
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  useEffect(() => {
    if (allExpenses.length === 0 && allIncome.length === 0) return;

    const filteredExpenses = allExpenses.filter(exp => {
        const d = exp.date.split('T')[0];
        return (!startDate || d >= startDate) && (!endDate || d <= endDate);
    });

    const filteredIncome = allIncome.filter(inc => {
        const d = inc.date.split('T')[0];
        return (!startDate || d >= startDate) && (!endDate || d <= endDate);
    });

    const combined = [...filteredExpenses, ...filteredIncome];
    combined.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
    setDisplayedTransactions(combined);

    const categoryMap = new Map<string, number>();
    filteredExpenses.forEach(exp => {
      const matchedCat = categories.find(c => c.id === exp.categoryId);
      const catName = matchedCat ? matchedCat.name : "Inne";
      const currentTotal = categoryMap.get(catName) || 0;
      categoryMap.set(catName, currentTotal + exp.amount);
    });
    setChartData(Array.from(categoryMap, ([name, value]) => ({ name, value })));

  }, [startDate, endDate, allExpenses, allIncome, categories]);

  const isExpense = (item: Expense | Income): item is Expense => {
    return (item as Expense).categoryId !== undefined;
  };

  if (isLoading) {
    return <div className="loading-container"><h2>Ładowanie...</h2></div>;
  }

  return (
    <div className="dashboard-container">
      <aside className="sidebar">
        <h2>MyBudget</h2>
        <nav>
          <Link to="/">Pulpit</Link>
          <Link to="/expenses">Wydatki</Link>
          <Link to="/income">Przychody</Link>
          <Link to="/settings">Ustawienia</Link>
        </nav>
        <div className="sidebar-footer">
          <button onClick={handleLogout}>Wyloguj się</button>
        </div>
      </aside>

      <main className="main-content">
        <div className="balance-card">
          <h3>Aktualne saldo konta</h3>
          <div className={`balance-amount ${balance < 0 ? 'negative' : ''}`}>
            {balance.toFixed(2)} {currency}
          </div>
        </div>

        <div className="date-filter-section">
          <label>Zakres od:</label>
          <input 
            type="date" 
            className="date-input"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
          <span className="date-separator">—</span>
          <label>do:</label>
          <input 
            type="date" 
            className="date-input"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
          <button 
             className="btn-reset-date"
             onClick={() => { setStartDate(''); setEndDate(getToday()); }}
          >
             Pokaż wszystko
          </button>
        </div>

        <div className="widgets-area">
          
          <div className="widget-card">
            <h3>Twoje wydatki</h3>
            <div className="chart-wrapper">
              {chartData.length > 0 ? (
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={chartData}
                      cx="50%"
                      cy="65%"
                      labelLine={true}
                      label={({ name, percent }: any) => `${name} ${(percent * 100).toFixed(0)}%`} 
                      outerRadius={110}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {chartData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip formatter={(value: any) => `${Number(value).toFixed(2)} ${currency}`} />
                    <Legend 
                      verticalAlign="bottom" 
                      height={36} 
                      wrapperStyle={{ paddingTop: '40px' }} 
                    />
                  </PieChart>
                </ResponsiveContainer>
              ) : (
                <div className="no-data-message">
                  Brak wydatków w tym terminie.
                </div>
              )}
            </div>
          </div>

          <div className="widget-card">
            <h3>Historia transakcji</h3>
            <ul className="transaction-list">
              {displayedTransactions.length > 0 ? (
                displayedTransactions.map((item) => {
                  const isItemExpense = isExpense(item);
                  let title = "";
                  let subTitle = "";
                  let amountClass = "";
                  let sign = "";

                  if (isItemExpense) {
                    const matchedCat = categories.find(c => c.id === item.categoryId);
                    title = matchedCat ? matchedCat.name : "Inne";
                    subTitle = item.note || "";
                    amountClass = "amount-red";
                    sign = "-";
                  } else {
                    title = (item as Income).source || "Przychód";
                    subTitle = "Wpływ środków";
                    amountClass = "amount-green";
                    sign = "+";
                  }

                  return (
                    <li key={item.id} className="transaction-item">
                      <div className="transaction-info">
                        <strong>{title}</strong>
                        <span>
                          {new Date(item.date).toLocaleDateString()} 
                          {subTitle ? ` • ${subTitle}` : ''}
                        </span>
                      </div>
                      
                      <div className={amountClass}>
                        {sign}{item.amount.toFixed(2)} {currency}
                      </div>
                    </li>
                  );
                })
              ) : (
                <p className="list-no-data-message">
                  Brak transakcji w tym okresie.
                </p>
              )}
            </ul>
          </div>

        </div>
      </main>
    </div>
  );
};

export default Dashboard;