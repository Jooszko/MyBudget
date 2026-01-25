import React, { useState } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, Link } from 'react-router-dom';
import { authService } from '../api/axios';
import { RegisterRequest } from '../types';
import '../styles/LoginPage.css'; 

const RegisterPage = () => {
  const { register, handleSubmit, formState: { errors } } = useForm<RegisterRequest>();
  const navigate = useNavigate();
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const onSubmit = async (data: RegisterRequest) => {
    setIsLoading(true);
    setErrorMessage(null);

    try {
      await authService.register(data);
      alert("Konto utworzone! Możesz się teraz zalogować.");
      navigate('/login');
    } catch (error: any) {
      setErrorMessage("Nie udało się utworzyć konta. Sprawdź dane lub spróbuj innej nazwy.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="login-container">
      <div className="login-box">
        <h2>Załóż konto</h2>
        
        {errorMessage && <div className="error-alert">{errorMessage}</div>}

        <form onSubmit={handleSubmit(onSubmit)}>
          
          <div className="form-group">
            <label>Nazwa użytkownika</label>
            <input 
              {...register("userName", { required: "Nazwa jest wymagana" })} 
            />
            {errors.userName && <span className="error-text">{errors.userName.message}</span>}
          </div>

          <div className="form-group">
            <label>Email</label>
            <input 
              type="email" 
              {...register("email", { required: "Email jest wymagany" })} 
            />
            {errors.email && <span className="error-text">{errors.email.message}</span>}
          </div>

          <div className="form-group">
            <label>Waluta</label>
            <select {...register("currency", { required: "Wybierz walutę" })} className="form-select">
              <option value="PLN">PLN</option>
              <option value="USD">USD</option>
              <option value="EUR">EUR</option>
            </select>
          </div>

          <div className="form-group">
            <label>Hasło</label>
            <input 
              type="password" 
              {...register("password", { 
                required: "Hasło jest wymagane",
                minLength: { value: 6, message: "Hasło musi mieć min. 6 znaków" }
              })} 
            />
            {errors.password && <span className="error-text">{errors.password.message}</span>}
          </div>

          <button type="submit" disabled={isLoading}>
            {isLoading ? 'Rejestracja...' : 'Zarejestruj się'}
          </button>
        </form>
        
        <p className="auth-footer">
          Masz już konto? <Link to="/login">Zaloguj się</Link>
        </p>
      </div>
    </div>
  );
};

export default RegisterPage;