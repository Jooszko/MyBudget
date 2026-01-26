import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import apiClient from '../api/axios';
import { Category } from '../types';
import '../styles/dashboard.css';
import '../styles/settings.css';

const SettingsPage = () => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [newCategoryName, setNewCategoryName] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const fetchCategories = async () => {
    try {
      const res = await apiClient.get<Category[]>('/api/Category');
      const sorted = res.data.sort((a, b) => a.name.localeCompare(b.name));
      setCategories(sorted);
    } catch (error) {
      console.error("Błąd pobierania kategorii:", error);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newCategoryName.trim()) return;

    try {
      await apiClient.post('/api/Category', { name: newCategoryName });
      setNewCategoryName(""); 
      fetchCategories(); 
    } catch (error) {
      alert("Nie udało się dodać kategorii.");
    }
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Czy na pewno usunąć tę kategorię?")) return;

    try {
      await apiClient.delete(`/api/Category/${id}`);
      setCategories(prev => prev.filter(c => c.id !== id));
    } catch (error) {
      alert("Nie udało się usunąć (być może jest używana w wydatkach?).");
    }
  };

  const handleLoadDefaults = async () => {
    if(!window.confirm("Dodać domyślne kategorie?")) return;
    setIsLoading(true);
    const defaults = ["Jedzenie", "Mieszkanie", "Transport", "Rozrywka", "Inne"];
    try {
      for (const name of defaults) {
        if (!categories.some(c => c.name.toLowerCase() === name.toLowerCase())) {
          await apiClient.post('/api/Category', { name });
        }
      }
      await fetchCategories();
      alert("Gotowe!");
    } catch (e) { 
        console.error(e); 
    } finally { 
        setIsLoading(false); 
    }
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
          <Link to="/income">Przychody</Link>
          <Link to="/settings" className="sidebar-link-active">Ustawienia</Link>
        </nav>
        <div style={{ marginTop: 'auto' }}>
          <button onClick={handleLogout}>Wyloguj się</button>
        </div>
      </aside>

      <main className="main-content">
        <h1>Ustawienia</h1>

        <div className="settings-card">
          <h3>Zarządzaj Kategoriami</h3>

          <form onSubmit={handleAdd} className="add-category-form">
            <input 
              type="text" 
              className="add-category-input"
              placeholder="Nowa kategoria (np. Hobby)..."
              value={newCategoryName}
              onChange={e => setNewCategoryName(e.target.value)}
            />
            <button type="submit" className="add-btn">Dodaj</button>
          </form>

          <ul className="categories-list">
            {categories.map(cat => (
              <li key={cat.id} className="category-item">
                <span>{cat.name}</span>
                <div className="cat-actions">
                  <button onClick={() => handleDelete(cat.id)} className="icon-btn btn-delete" title="Usuń">🗑️</button>
                </div>
              </li>
            ))}
            {categories.length === 0 && <p className="empty-cat-message">Brak kategorii.</p>}
          </ul>

          <div className="defaults-section">
             <button onClick={handleLoadDefaults} disabled={isLoading} className="load-defaults-btn">
               {isLoading ? "Ładowanie..." : "Załaduj domyślne kategorie (jeśli brakuje)"}
             </button>
          </div>

        </div>
      </main>
    </div>
  );
};

export default SettingsPage;