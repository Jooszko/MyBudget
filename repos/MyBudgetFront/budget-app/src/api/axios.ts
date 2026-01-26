import axios from 'axios';
import { User, LoginRequest, RegisterRequest } from '../types';


const apiClient = axios.create({
  baseURL: 'https://localhost:7230', 
  headers: {
    'Content-Type': 'application/json',
  },
});


apiClient.interceptors.request.use(
  (config) => {
    const userData = localStorage.getItem('user');
    if (userData) {
      const user: User = JSON.parse(userData);
      if (user.token) {
        config.headers.Authorization = `Bearer ${user.token}`;
      }
    }
    return config;
  },
  (error) => Promise.reject(error)
);


export const authService = {
  login: async (data: LoginRequest) => {

    const response = await apiClient.post<User>('/api/Account/login', data);
    
    if (response.data.token) {
      localStorage.setItem('user', JSON.stringify(response.data));
    }
    return response.data;
  },

  register: async (data: RegisterRequest) => {
    const response = await apiClient.post('/api/Account/register', data);
    return response.data;
  },

  logout: () => {
    localStorage.removeItem('user');
    window.location.reload();
  },
};

export default apiClient;