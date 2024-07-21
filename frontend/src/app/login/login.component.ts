import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SharedService } from '../shared.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  loginObj: Login;

  constructor(private http: HttpClient,private router: Router, private sharedService: SharedService) {
    this.loginObj = new Login('', '');
  
  }

  
  onSubmit() {
    
    let body = new FormData();
    body.append('username', this.loginObj.username);
   
    body.append('password', this.loginObj.password);
    this.sharedService.setSharedVariable(this.loginObj.username);
    console.log(body);
    this.http.post('http://localhost:8000/api/Todo/Login', body).subscribe((data: any) => {
      console.log(data);
      if(data.length > 0){
        console.log("Login Successful");
        this.router.navigateByUrl('home');
      }
      else{
        alert("Login Failed, Invalid Credentials");
        console.log("Login Failed");
      }
    }
    );

  }
  toRegister(){
    this.router.navigateByUrl('register');
  }
}

export class Login{
  username: string;
  password: string;

  constructor(EmailId: string, Password: string){
    this.username = EmailId;
    this.password = Password;
  }
  
}
