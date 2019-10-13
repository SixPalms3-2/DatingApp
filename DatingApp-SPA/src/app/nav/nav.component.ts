import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};

    constructor(private authService: AuthService) { }

  ngOnInit() {
  }


  login() {
    this.authService.login(this.model)
    .subscribe(next => { console.log('Logged in successfully'); },
               error => { console.log('Failed to login'); } );
  }

  loggedIn() {
    const token = localStorage.getItem('token');

    return !!token; // If token in empty return value will be false if token is NOT empty, the return value will be true
  }

  logout() {
    localStorage.removeItem('token');
    console.log('logged out');
  }

}
