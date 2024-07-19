import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, HttpClientModule, FormsModule, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'frontend';

  tasks:any = []; 
  task = "";

  API_URL = 'http://localhost:8000/api/Todo';

  constructor(private http: HttpClient) {}
  ngOnInit(){
      this.get_tasks();
    }
    
    get_tasks(){
      this.http.get(`${this.API_URL}/get_tasks`).subscribe((data) => {
        this.tasks = data;
        
      });
    }

    add_task(){
     let body = new FormData();
      body.append('task', this.task);
      this.http.post(`${this.API_URL}/add_task`, body).subscribe((data) => {
        
        this.task = "";
        this.get_tasks();
      });
    }

    delete_task(id:any){
      let body = new FormData();
      body.append('id', id);
      this.http.post(`${this.API_URL}/delete_task`, body).subscribe((data) => {
        this.get_tasks();
      });
    }
}
