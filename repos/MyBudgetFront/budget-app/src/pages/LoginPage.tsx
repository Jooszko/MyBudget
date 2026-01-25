import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, Link } from 'react-router-dom';
import { authService } from '../api/axios'; 
import { LoginRequest } from '../types';
import '../styles/LoginPage.css';

const LoginPage = () => {
  const { register, handleSubmit, formState: { errors } } = useForm<LoginRequest>();
  const navigate = useNavigate();
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const onSubmit = async (data: LoginRequest) => {
    setIsLoading(true);
    setErrorMessage(null);
    
    try {
      await authService.login(data);
      navigate('/'); 
    } catch (error: any) {
      setErrorMessage("Nie udało się zalogować. Sprawdź email i hasło.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-box">
        <h2>Zaloguj się</h2>
        
        {errorMessage && <div className="error-alert">{errorMessage}</div>}

        <form onSubmit={handleSubmit(onSubmit)}>
          
          <div className="form-group">
            <label>Email</label>
            <input 
              type="email" 
              {...register("email", { required: "Email jest wymagany" })} 
            />
            {errors.email && <span className="error-text">{errors.email.message}</span>}
          </div>

          <div className="form-group">
            <label>Hasło</label>
            <input 
              type="password" 
              {...register("password", { required: "Hasło jest wymagane" })} 
            />
            {errors.password && <span className="error-text">{errors.password.message}</span>}
          </div>

          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Logowanie...' : 'Zaloguj się'}
          </button>
        </form>
        
        <p className="auth-footer">
            Nie masz konta? <Link to="/register">Zarejestruj się</Link>
        </p>
      </div>
    </div>
  );
};

export default LoginPage;