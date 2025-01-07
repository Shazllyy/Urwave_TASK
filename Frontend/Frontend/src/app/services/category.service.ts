import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { CategoryRequestDTO } from '../interfaces/Category';
import { CategoryResponseDTO } from '../interfaces/Category';
import { ProductResponseDTO } from '../interfaces/product.model.model';


@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private apiUrl = 'https://localhost:7113/api/categories';

  private categoriesSubject = new BehaviorSubject<CategoryResponseDTO[]>([]);
  public categories$ = this.categoriesSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Function to get the authentication token from sessionStorage
  private getAuthToken(): string | null {
    return sessionStorage.getItem('token'); // Assuming the token is stored in sessionStorage
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

  // Get categories
  getCategories(): Observable<CategoryResponseDTO[]> {
    const headers = this.createHeaders();
    return this.http
      .get<CategoryResponseDTO[]>(this.apiUrl, { headers })
      .pipe(
        map((categories) => categories.map((c) => this.transformCategory(c)))
      );
  }

  // Get category by ID
  getCategoryById(id: string): Observable<CategoryResponseDTO> {
    const headers = this.createHeaders();
    return this.http
      .get<CategoryResponseDTO>(`${this.apiUrl}/${id}`, { headers })
      .pipe(map((category) => this.transformCategory(category)));
  }

  // Create a new category
  createCategory(categoryDto: CategoryRequestDTO): Observable<CategoryResponseDTO> {
    const headers = this.createHeaders();
    return this.http
      .post<CategoryResponseDTO>(this.apiUrl, categoryDto, { headers })
      .pipe(map((category) => this.transformCategory(category)));
  }

  // Update an existing category


  updateCategory(id: string, category: CategoryRequestDTO): Observable<void> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${this.getAuthToken()}`, // Include the Authorization token if required
    });

    console.log(category);
    // Send the category data in the body of the PUT request
    return this.http.put<void>(`${this.apiUrl}/${id}`, category, { headers });
  }
  // Delete a category
  deleteCategory(id: string, newCategoryId?: string): Observable<void> {
    const headers = this.createHeaders();
    const url = newCategoryId
      ? `${this.apiUrl}/${id}?newCategoryId=${newCategoryId}`
      : `${this.apiUrl}/${id}`;
    return this.http.delete<void>(url, { headers });
  }

  // Optionally, transform category data (e.g., format dates)
  private transformCategory(category: CategoryResponseDTO): CategoryResponseDTO {
    category.createdDate = new Date(category.createdDate).toLocaleString();
    category.updatedDate = new Date(category.updatedDate).toLocaleString();
    return category;
  }

  // Move products to another category when deleting
  moveProductsWhenDeletingCategory(oldCategoryId: string, newCategoryId: string): Observable<void> {
    const headers = this.createHeaders();
    return this.http.put<void>(`${this.apiUrl}/move-products`, { oldCategoryId, newCategoryId }, { headers });
  }
  getSubcategoriesByCategoryId(id: string): Observable<CategoryResponseDTO[]> {
    const headers = this.createHeaders();
    return this.http
      .get<CategoryResponseDTO[]>(`${this.apiUrl}/${id}/subcategories`, { headers })
      .pipe(
        map((subcategories) => subcategories.map((c) => this.transformCategory(c)))
      );
  }
  getProductsByCategoryId(categoryId: string): Observable<ProductResponseDTO[]> {
    const headers = this.createHeaders();
    return this.http.get<ProductResponseDTO[]>(`${this.apiUrl}/${categoryId}/products`, { headers });
  }

}
