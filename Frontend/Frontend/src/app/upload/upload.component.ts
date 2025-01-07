import { Component } from '@angular/core';
import { CsrfService } from '../services/csrf.service';
import { catchError } from 'rxjs/operators';
import { of } from 'rxjs';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent {
  fileToUpload: File | null = null;
  message: string = '';
  isLoading: boolean = false;  // To track the loading state

  constructor(private csrfService: CsrfService) {}

  // Method to handle CSRF token retrieval and file upload
  onRetrieveToken(): void {
    this.isLoading = true;
    this.csrfService.retrieveAndStoreCsrfToken().subscribe({
      next: (response) => {
        this.message = 'CSRF Token successfully retrieved and stored!';
        this.isLoading = false;
      },
      error: (error) => {
        this.message = 'Failed to retrieve CSRF Token: ' + error.message;
        this.isLoading = false;
      }
    });
  }

  // Method to handle file selection
  onFileSelected(event: any): void {
    this.fileToUpload = event.target.files[0];
    if (this.fileToUpload) {
      this.message = 'File selected: ' + this.fileToUpload.name;
    }
  }

  onUpload(): void {
    if (!this.fileToUpload) {
      this.message = 'Please select a file first.';
      console.log('No file selected');
      return;
    }
  
    // Retrieve CSRF token from sessionStorage
    const csrfToken = 'CfDJ8CeNEHSbd6tFg6RZ8rOX9236z3OJXeqwTMa21TKQFWiL1NRvMCIy-Mu1887X_FE2Hmsv5-vMakbcBtEtmHBRxQsaC0-mywQaNNBzrb1gpn4NGGT8fdobe1tbS9orsHvkYpi61682OeIOgmqaXo6wbr0';
    
    if (!csrfToken) {
      this.message = 'CSRF token is missing. Please try again.';
      console.log('CSRF token is missing');
      return;
    }
  
    this.isLoading = true;
  
    // Call the uploadImage method directly
    this.csrfService.uploadImage(this.fileToUpload!).subscribe({
      next: (uploadResponse) => {
        this.message = 'File uploaded successfully!';
        console.log('Upload response:', uploadResponse); // Log successful upload response
        this.isLoading = false;
      },
      error: (uploadError) => {
        this.message = 'File upload failed: ' + uploadError.message;
        console.log('Upload error:', uploadError); // Log upload error
        this.isLoading = false;
      }
    });
  }
  
}
