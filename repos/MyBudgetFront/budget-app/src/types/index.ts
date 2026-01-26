
export interface User {
  token: string;
  userName: string;
  currency: string;
}


export interface LoginRequest {
  email?: string;
  password?: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  userName: string;
  currency: string;
}


export interface Expense {
  id: string;          
  amount: number;
  date: string;
  note?: string;
  categoryId: string;   
  categoryName?: string;       
}

export interface CreateExpenseRequest {
  categoryName: string;
  amount: number;
  date: string;
  note?: string;
}

export interface UpdateExpenseRequest {
  categoryName: string;
  amount: number;
  date: string;
  note?: string;
}


export interface Income {
  id: string;
  amount: number;
  date: string;
  source?: string;
}

export interface CreateIncomeRequest {
  amount: number;
  date: string;
  source?: string;
}

export interface Category {
  id: string;
  name: string;
}