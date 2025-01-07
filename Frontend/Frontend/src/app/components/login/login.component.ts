// login.component.ts
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CardModule,
    InputTextModule,
    FormsModule,
    PasswordModule,
    ButtonModule,
    RouterLink,
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  login = {
    userName: '',
    password: '',
  };

  private authService = inject(AuthService);
  private router = inject(Router);
  private messageService = inject(MessageService);

  onLogin() {
    const { userName, password } = this.login;
    this.authService.loginUser({ username: userName, password }).subscribe({
      next: (response) => {
        console.log('Server Response:', response);
  
        // Store JWT Token in sessionStorage
        sessionStorage.setItem('token', response.token);
  
        // Retrieve anti-forgery token from the response (assuming it's sent in response headers)
        const antiForgeryToken = response.antiForgeryToken;  // Adjust this according to how the token is returned
  
        // Save the anti-forgery token (e.g., in sessionStorage)
        if (antiForgeryToken) {
          sessionStorage.setItem('X-XSRF-TOKEN', antiForgeryToken);
        }
  
        // Navigate to home after successful login
        this.router.navigate(['home']);
      },
      error: (err) => {
        console.error('Login Error:', err);
        this.messageService.add({
          severity: 'error',
          summary: 'Login Failed',
          detail: 'Invalid credentials. Please try again.',
        });
      },
    });
  }
  
}
