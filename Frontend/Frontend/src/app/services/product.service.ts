import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private apiUrl = 'https://localhost:7113/api/products';

  private productsSubject = new BehaviorSubject<any[]>([]);
  public products$ = this.productsSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Function to get the authentication token from sessionStorage
  private getAuthToken(): string | null {
    return sessionStorage.getItem('token'); // Assuming the token is stored in sessionStorage
  }
  private getAntiForgeryToken(): string | null {
    return sessionStorage.getItem('X-XSRF-TOKEN'); // Assuming the anti-forgery token is stored in sessionStorage
  }
  // Function to create the authorization headers
  private createHeaders(): HttpHeaders {
    const token = this.getAuthToken();
    let headers = new HttpHeaders();
    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }
    return headers;
  }

  // Get products with filters, sorting, and pagination
  getProducts(
    first: number,
    rows: number,
    category: string,
    priceRange: string,
    status: string,
    sortField: string,
    sortOrder: number
  ): Observable<any[]> {
    const params = new HttpParams()
      .set('pageNumber', (first / rows + 1).toString())
      .set('pageSize', rows.toString())
      .set('category', category)
      .set('priceRange', priceRange)
      .set('status', status)
      .set('sortField', sortField)   // Add sortField to request
      .set('sortOrder', sortOrder.toString()); // Add sortOrder to request

    const headers = this.createHeaders(); // Set headers with the token

    return this.http
      .get<any[]>(this.apiUrl, { headers, params }) // Return observable directly
      .pipe(
        map((response) => response.map((p: any) => this.transformProduct(p))
      ) // Optionally, transform data if needed
      );
  }

  // Transform product data if needed (for example, formatting dates or adding properties)
  private transformProduct(product: any): any {
    // Example: Format date or add other properties
    product.CreatedDateFormatted = new Date(product.CreatedDate).toLocaleString();
    return product;
  }

  // Get a single product by ID (for use in editing)
  getProductById(id: string): Observable<any> {
    const headers = this.createHeaders(); // Set headers with the token
    return this.http.get<any>(`${this.apiUrl}/${id}`, { headers });
  }

  // Create a new product
  createProduct(productDto: any): Observable<any> {
    console.log("service log");
    const headers = this.createHeaders(); // Set headers with the token
    return this.http.post<any>(this.apiUrl, productDto, { headers });
  }
  createProductWithImage(formData: FormData): Observable<any> {
    const headers = this.createHeaders(); 
    return this.http.post<any>(this.apiUrl, formData, { headers });
  }
  
  


  

  // Update an existing product
  updateProduct(id: string, productDto: any): Observable<any> {

    const headers = this.createHeaders(); // Set headers with the token
    return this.http.put<any>(`${this.apiUrl}/${id}`, productDto, { headers });
  }

  // Delete a product by ID
  deleteProduct(id: string): Observable<any> {
    const headers = this.createHeaders(); // Set headers with the token
    return this.http.delete<any>(`${this.apiUrl}/${id}`, { headers });
  }

  // Batch delete products
  batchDeleteProducts(ids: string[]): Observable<any> {
    const headers = this.createHeaders(); // Set headers with the token
    return this.http.delete<any>(`${this.apiUrl}/batch`, { body: ids, headers });
  }
  uploadProductImage(productId: string, file: File): Observable<any> {
    const antiForgeryToken = this.getAntiForgeryToken(); // Retrieve the anti-forgery token from sessionStorage
    let headers = new HttpHeaders();
  
    // Add Anti-Forgery Token to headers if available
    if (antiForgeryToken) {
      headers = headers.set('X-XSRF-TOKEN', antiForgeryToken);
    }
  
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
  
    return this.http.post<any>(`${this.apiUrl}/${productId}/upload-image`, formData, {
      headers: headers.set('Content-Type', 'multipart/form-data'),
    });
  }
  
}
