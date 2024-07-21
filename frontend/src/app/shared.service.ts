import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  private sharedVariable: string | null = null;

  setSharedVariable(value: string | null) {
    this.sharedVariable = value;
    if (value) {
      localStorage.setItem('username', value);
    } else {
      localStorage.removeItem('username');
    }
  }

  getSharedVariable() {
    if (!this.sharedVariable) {
      this.sharedVariable = localStorage.getItem('username');
    }
    return this.sharedVariable;
  }
}