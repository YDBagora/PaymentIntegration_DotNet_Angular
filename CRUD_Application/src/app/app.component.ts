import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
// import { HttpClient } from '@angular/common/http';
// import { EmployeeComponent } from './Components/employee/employee.component';

@Component({
  selector: 'app-root',
  // standalone: true,
  imports: [CommonModule,RouterOutlet ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'CRUD_Application';
}
