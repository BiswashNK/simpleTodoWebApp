import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClient, HttpParams } from '@angular/common/http';
import { RouterOutlet, Router } from '@angular/router';
import { LoginComponent } from '../login/login.component';
import { SharedService } from '../shared.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterOutlet, LoginComponent],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  title = 'Task Management';
  tasks: any[] = [];
  task = '';
  errorMessage = '';
  username: string | null = null;
  API_URL = 'http://localhost:8000/api/Todo';

  constructor(private http: HttpClient, private sharedService: SharedService, private router: Router) {}

  ngOnInit() {
    this.username = this.sharedService.getSharedVariable();
    if (this.username) {
      this.get_tasks();
    } else {
      this.router.navigateByUrl('login');
    }
  }

  get_tasks() {
    const username = this.sharedService.getSharedVariable();
    if (username) {
      const params = new HttpParams().set('username', username);

      this.http.get(`${this.API_URL}/get_tasks`, { params }).subscribe((data: any) => {
        this.tasks = data;
      });
    }
  }

  add_task() {
    if (this.task.trim().length === 0) {
      this.errorMessage = 'Task cannot be empty!';
      return;
    }

    const username = this.sharedService.getSharedVariable();
    if (username) {
      let body = new FormData();
      body.append('task', this.task);
      body.append('username', username);
      this.http.post(`${this.API_URL}/add_task`, body).subscribe(() => {
        this.task = '';
        this.errorMessage = '';
        this.get_tasks();
      });
    }
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
      let body = new FormData();
      body.append('id', id);
      body.append('completed', task.completed.toString());
      this.http.post(`${this.API_URL}/update_task`, body).subscribe(() => {
        this.get_tasks();
      });
    }
  }

  logout() {
    this.sharedService.setSharedVariable(null);
    this.router.navigateByUrl('login');
  }
}