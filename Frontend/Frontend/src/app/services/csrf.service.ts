import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators'; // <-- Import the tap operator

@Injectable({
  providedIn: 'root'
})
export class CsrfService {
  private apiUrl = 'https://localhost:7113/api/csrf-token'; // Change this to your API URL
  private uploadUrl = 'https://localhost:7113/api/upload'; // URL for file upload endpoint
  
  constructor(private http: HttpClient) {}

  // Method to fetch CSRF token from the backend and store it in sessionStorage
  retrieveAndStoreCsrfToken(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}`).pipe(
      tap(response => {
        this.storeCsrfToken(response.token);
      })
    );
  }

  // Store CSRF token in sessionStorage
  storeCsrfToken(token: string): void {
    sessionStorage.setItem('csrfToken', token);
  }

  // Get the stored CSRF token from sessionStorage
  getStoredCsrfToken(): string | null {
    return sessionStorage.getItem('csrfToken');
  }

  uploadImage(file: File): Observable<any> {
    // Set the CSRF token value directly
    const csrfToken = 'CfDJ8CeNEHSbd6tFg6RZ8rOX9236z3OJXeqwTMa21TKQFWiL1NRvMCIy-Mu1887X_FE2Hmsv5-vMakbcBtEtmHBRxQsaC0-mywQaNNBzrb1gpn4NGGT8fdobe1tbS9orsHvkYpi61682OeIOgmqaXo6wbr0';
  
    // Check if CSRF token is missing (for safety reasons)
    if (!csrfToken) {
      throw new Error('CSRF token is missing');
    }
  
    // Debug: Log the token before sending the request
    console.log('Sending CSRF token:', csrfToken);
  
    const formData: FormData = new FormData();
    formData.append('file', file, file.name);
  
    // Create headers and add CSRF token in the X-CSRF-TOKEN header
    const headers = new HttpHeaders({
      'X-CSRF-TOKEN': csrfToken  // Attach the token to the header
    });
  
    // Debug: Log the headers being sent
    console.log('Request Headers:', headers);
  
    return this.http.post<any>(this.uploadUrl, formData, { headers });
  }
  
  
}
