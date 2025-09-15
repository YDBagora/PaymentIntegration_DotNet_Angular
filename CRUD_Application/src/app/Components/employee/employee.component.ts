import { Component, ElementRef, inject, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Employee } from '../../Models/employee';
import { EmployeeService } from '../../Services/employee.service';
declare var paypal: any; 
declare var Razorpay: any; 

@Component({
  selector: 'app-employee',
  imports: [ReactiveFormsModule],
  templateUrl: './employee.component.html',
  styleUrl: './employee.component.css'
})
export class EmployeeComponent implements OnInit  {

  @ViewChild('myModel') model: ElementRef | undefined;
  @ViewChild('myModels') models: ElementRef | undefined;
  employeeList: Employee[] = [];
  employeeService = inject(EmployeeService);
  formValue: any;

  employeeForm: FormGroup = new FormGroup({});
  item: any;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.setFromState();
    this.getEmployees();
    this.item = this.employeeList;
  }

  // To Open the Add or Update Employee Model function
  openModel() {
    const empModel = document.getElementById('myModal');
    if (empModel != null) {
      empModel.style.display = "block";
    }
  }

    openModels() {
    const empModel = document.getElementById('myModals');
    if (empModel != null) {
      empModel.style.display = "block";
    }
  }

  // To Close Add or Update Employee Model function
  closeModel() {
    this.setFromState();
    if (this.model != null) {
      this.model.nativeElement.style.display = 'none';
    }
  }

   closeModels() {
    this.setFromState();
    if (this.models != null) {
      this.models.nativeElement.style.display = 'none';
    }
  }

  ngAfterViewInit(): void {
    this.loadPayPalButtons();
  }

  loadPayPalButtons() {
    paypal.Buttons({
      style: {
        layout: 'vertical',
        color: 'gold',
        shape: 'rect',
        label: 'paypal'
      },
      createOrder: (data: any, actions: any) => {
        return actions.order.create({
          purchase_units: [{
            amount: {
              value: '10.00'   // 💲 set dynamic value if needed
            }
          }]
        });
      },
      onApprove: (data: any, actions: any) => {
        return actions.order.capture().then((details: any) => {
          alert('Transaction completed by ' + details.payer.name.given_name);
          console.log(details);
        });
      },
      onError: (err: any) => {
        console.error(err);
        alert("Something went wrong with PayPal payment.");
      }
    }).render('#paypal-button-container'); // Render PayPal button inside container
  }

  // The function is empty fileds and validates it
  setFromState() {
    this.employeeForm = this.fb.group({
      id: [0],
      name: ['', [Validators.required]],
      email: ['', [Validators.required]],
      phone: ['', [Validators.required]],
      salary: ['', [Validators.required]],
    })
  }

  // This Function is called when the form is Submited for Add or Update the Employee
  onSubmit() {
    console.log(this.employeeForm.value);
    if (this.employeeForm.invalid) {
      alert("Please Fill the Form");
      return;
    }

    if (this.employeeForm.value.id == 0) {
      this.formValue = this.employeeForm.value;
      this.formValue.salary = parseFloat(this.formValue.salary);
      this.employeeService.addEmployee(this.formValue).subscribe(
        (res) => {
          alert("Employee Added Successfully");
          this.getEmployees();
          this.employeeForm.reset();
          this.closeModel();
        },
        (err) => {
          console.error("Error adding employee:", err);
          alert("Failed to add employee. Please check the data.");
        })
    }
    else {
      this.formValue = this.employeeForm.value;
      this.formValue.salary = parseFloat(this.formValue.salary);
      this.employeeService.updateEmployee(this.formValue).subscribe(
        (res) => {
          alert("Employee Updated Successfully");
          this.getEmployees();
          this.employeeForm.reset();
          this.closeModel();
        },
        (err) => {
          console.error("Error adding employee:", err);
          alert("Failed to add employee. Please check the data.");
        })
    }
  }

  // This Function is used to display all the employees present in the data base
  getEmployees() {
    this.employeeService.getAllEmployee().subscribe((res) => (
      this.employeeList = res
    ));
  }

  // This Function is used to open the model and fill the selected employees in the input
  updateEmployee(item: Employee) {
    this.openModel();
    this.employeeForm.patchValue(item);
  }

  // This Function is use to Delete the Employee form Database; 
  removeEmployees(item: Employee) {
    const isConfirm = confirm("Are You Sure to Delete this employee : " + item.name);
    if (isConfirm) {
      this.employeeService.deletEmployee(item.id).subscribe((res) => {
        alert('Employee Deleted Successfully');
        this.getEmployees();
      })
    }
  }

  paymentIntegration(item: any){
      this.openModels();
    document.getElementById("paypal-button-container")!.innerHTML = ""; 

    paypal.Buttons({
      createOrder: (data: any, action: any) => {
        return action.order.create({
          purchase_units: [{
            amount: {
              value: item.salary.toString()  
            }
        }]
        })
      },
      onApprove: (data: any, actions: any) => {
      return actions.order.capture().then((details: any) => {
        alert(`Payment of $${item.salary} completed by ${details.payer.name.given_name}`);
      });
    }
    }).render('#paypal-button-container');
  }

   payWithRazorpay(item: any) {
    const options = {
      key: "rzp_test_RGhzCZg731Q1gy", 
      amount: item.salary * 100,  
      currency: "INR",
      name: "YDBAGORA",
      description: "Test Transaction",
      image: "https://yourcompanylogo.com/logo.png",  // optional
      handler: (response: any) => {
        alert("Payment successful. Razorpay Payment");
        console.log(response);

        // 👉 Call your .NET Core backend to save payment info
        // this.http.post("https://localhost:7020/api/payment/save", response).subscribe(...)
      },
      prefill: {
        name: "YD",
        email: "YD@example.com",
        contact: "9999999999"
      },
      theme: {
        color: "#3399cc"
      }
    };

    const rzp1 = new Razorpay(options);
    rzp1.open();
  }
}
