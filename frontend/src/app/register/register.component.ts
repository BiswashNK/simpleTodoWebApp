import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterOutlet, Router } from '@angular/router';
import { LoginComponent } from '../login/login.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, HttpClientModule, RouterOutlet, LoginComponent],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerObj: Register;

  constructor(private http: HttpClient, private router: Router) {
    this.registerObj = new Register('', '', '');
  }

  onSubmit() {
    let body = new FormData();
    body.append('username', this.registerObj.username);
    body.append('password', this.registerObj.password);
    body.append('email', this.registerObj.email);
    console.log(body);
    this.http.post('http://localhost:8000/api/Todo/Register', body).subscribe((data: any) => {
      console.log(data);
      if (data.message === "Registered Successfully") {
        alert("Registration Successful");
        this.router.navigateByUrl('login');
      } else {
        alert("Registration Failed, User already exists");
        console.log("Registration Failed");
      }
    },
    (error) => {
      console.log(error);
      alert("Registration Failed, User already exists");
    }
  
  );
  }

  toLogin() {
    this.router.navigateByUrl('login');
  }
}

export class Register {
  username: string;
  password: string;
  email: string;

  constructor(EmailId: string, Password: string, Email: string) {
    this.username = EmailId;
    this.password = Password;
    this.email = Email;
  }
}