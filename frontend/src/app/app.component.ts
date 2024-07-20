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
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'frontend';

  tasks: any[] = []; 
  task = "";
  errorMessage = "";

  API_URL = 'http://localhost:8000/api/Todo';

  constructor(private http: HttpClient) {}

  ngOnInit() {
    this.get_tasks();
  }
    
  get_tasks() {
    this.http.get(`${this.API_URL}/get_tasks`).subscribe((data: any) => {
      this.tasks = data;
    });
  }

  add_task() {
    if (this.task.trim().length === 0) {
      this.errorMessage = 'Task cannot be empty!';
      return;
    }

    let body = new FormData();
    body.append('task', this.task);
    this.http.post(`${this.API_URL}/add_task`, body).subscribe(() => {
      this.task = "";
      this.errorMessage = "";
      this.get_tasks();
    });
  }

  delete_task(id: any) {
    let body = new FormData();
    body.append('id', id);
    this.http.post(`${this.API_URL}/delete_task`, body).subscribe(() => {
      this.get_tasks();
    });
  }

  toggle_task(id: any) {
    let task = this.tasks.find(t => t.id === id);
    if (task) {
      task.completed = !task.completed;
      // Assuming you have an API endpoint to update the task
      let body = new FormData();
      body.append('id', id);
      body.append('completed', task.completed);
      this.http.post(`${this.API_URL}/update_task`, body).subscribe(() => {
        this.get_tasks();
      });
    }
  }
}