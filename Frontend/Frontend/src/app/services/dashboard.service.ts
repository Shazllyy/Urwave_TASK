import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

interface DashboardData {
  TotalProducts: number;
  TotalCategories: number;
  ProductsPerCategory: any[];
  LowStockProducts: any[];
  RecentActivities: any[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private baseUrl = 'https://localhost:7113/api/dashboard'; // Replace with your backend API URL

  constructor(private http: HttpClient) {}

  // Method to retrieve the Authorization token from sessionStorage or wherever it is stored
  private getAuthToken(): string | null {
    // Retrieve the token from sessionStorage using the correct key ('token')
    return sessionStorage.getItem('token'); // 'token' is the key used for storing the token
  }

  // Method to create HTTP headers with the Authorization token
  private createAuthHeaders(): HttpHeaders {
    const token = this.getAuthToken();
    console.log('Token from sessionStorage:', token);  // Log the token to check its value
    
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`); // Send token in Bearer scheme
    } else {
      console.warn('No token found in sessionStorage!');
    }
    return headers;
  }

  // Get Dashboard Metrics (with Authorization header)
  getDashboardMetrics(): Observable<DashboardData> {
    const headers = this.createAuthHeaders();
    return this.http.get<any>(`${this.baseUrl}`, { headers });
  }

  // Get Products Per Category (with Authorization header)
  getProductsPerCategory(): Observable<any[]> {
    const headers = this.createAuthHeaders();
    return this.http.get<any[]>(`${this.baseUrl}/products-per-category`, { headers });
  }

  // Get Low Stock Products (with Authorization header)
  getLowStockProducts(threshold: number): Observable<any[]> {
    const headers = this.createAuthHeaders();
    return this.http.get<any[]>(`${this.baseUrl}/low-stock/${threshold}`, { headers });
  }

  // Get Recent Activities (with Authorization header)
  getRecentActivities(): Observable<any[]> {
    const headers = this.createAuthHeaders();
    return this.http.get<any[]>(`${this.baseUrl}/recent-activities`, { headers });
  }
}
