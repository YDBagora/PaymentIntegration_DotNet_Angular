import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Employee } from '../Models/employee';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  // ASP.NET Web API
  private apiUrl = 'https://localhost:7020/api/Employees'
  constructor() { }

  http = inject(HttpClient)

  // Service Method to get all the Employees using GET method
  getAllEmployee(){
    return this.http.get<Employee[]>('https://localhost:7020/api/Employees/GetAllEmployees');
  }

  // Service Method to add new Employee using POST method
  addEmployee(data: any) {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(this.apiUrl, data, { headers });
  }

  // Service Method to update Employee using PUT method
  updateEmployee(employee: Employee){
    return this.http.put(`${this.apiUrl}/${employee.id}`, employee);
  }

  //  Service Method to delete Employee using DELETE method
  deletEmployee(id: number){
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  createPayment(amount: number){
    return this.http.post<any>(`${this.apiUrl}/create-payment?amount=${amount}`, {});
  }

  executePayment(paymentId: string, payerId: string){
    return this.http.get<any>(`${this.apiUrl}/executepayment?paymentId=${paymentId}&payerId=${payerId}`);
  }
}