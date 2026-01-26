
import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';


import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import Dashboard from './pages/Dashboard';       
import ExpensesPage from './pages/ExpensesPage'; 
import SettingsPage from './pages/SettingsPage';
import IncomePage from './pages/IncomePage';


const PrivateRoute = ({ children }: { children: React.ReactNode }) => {
  const user = localStorage.getItem('user');
  
  return user ? <>{children}</> : <Navigate to="/login" />;
};

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route 
          path="/" 
          element={
            <PrivateRoute>
              <Dashboard />  
            </PrivateRoute>
          } 
        />

        <Route 
          path="/expenses" 
          element={
            <PrivateRoute>
              <ExpensesPage />
            </PrivateRoute>
          } 
        />

        <Route 
          path="/income" 
          element={
            <PrivateRoute>
              <IncomePage />
            </PrivateRoute>
          } 
        />

        <Route 
          path="/settings" 
          element={<PrivateRoute><SettingsPage /></PrivateRoute>} 
        />
        
        <Route path="*" element={<Navigate to="/" />} />

      </Routes>
    </BrowserRouter>
  );
}

export default App;