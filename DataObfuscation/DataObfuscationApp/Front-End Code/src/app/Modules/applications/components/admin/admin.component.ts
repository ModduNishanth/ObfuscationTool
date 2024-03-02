import { Component, OnInit } from '@angular/core';

import { ServiceApiService } from '../../../../service-api.service';

import { ToastrService } from 'ngx-toastr';

import { log } from 'console';
import { NgxUiLoaderService } from 'ngx-ui-loader';

@Component({
  selector: 'app-admin',

  templateUrl: './admin.component.html',

  styleUrls: ['./admin.component.css'],
})
export class AdminComponent implements OnInit {
  constructor(
    private apiService: ServiceApiService,
    private loder: NgxUiLoaderService,
    private toastMessage: ToastrService
  ) {}

  selectAllChecked: boolean = false;

  ngOnInit(): void {
    this.CallProjectList();
    this.Getfunction();
  }
  individualChecked: boolean[] = [];
  isAllChecked: boolean = false;
  colunmnNames: any[] = [];
  Functionlist: any;
  ProjectList: any;
  selectedProjectvalue: any;
  selectedId: any;
  isChecked: boolean = true;
  functionDefinition: any;
  FunctionName: any;
  functionNames: string[] = [];
  fnadded: any;
  selectedFunctionNames: string[] = [];
  datatype = [{ value: 'MYSQL' }, { value: 'SQLSERVER' }, { value: 'ORACLE' }];
  isColumnNamesEmpty: boolean = true;  // Boolean to be toggled based on the array length
  isNotificationVisible: boolean = false;
page : number =1;


  // Getfunction(data: any) {
  //   this.apiService.getFunctionListt(data).subscribe({
  //     next: (result) => {
  //       this.Functionlist = result;
  //       this.FunctionName = result;
  //       console.log("Hey", this.FunctionName);
  //       console.log(' function list', this.Functionlist);
  //       this.addFunction()
  //     },
  //     error: () => {
  //       console.log("Invalid Request")
  //     }
  //   })
  // }

  addFunction() {
    this.apiService.addFunction().subscribe({
      next: (result) => {
        this.fnadded = result;
        // Ensure that Functionlist is defined and accessible
        console.log('Hey', this.fnadded);
        console.log('Function list', this.Functionlist);

      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }

  getFunctionQuery(data: any) {
    this.functionDefinition = data;
  }

  onSelected(data: any) {
    this.selectedId = data;

    this.submitDataBasetype(this.selectedId);
  }

  CallProjectList() {
    this.apiService.getProjectList().subscribe({
      next: (result:any) => {
        this.ProjectList = result.filter((project:any) => project.testConnection === 1);
        console.log(' Project list', this.ProjectList);
        // this.CallTable()
      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }

  submitFunctionData() {
    this.loder.start();
    console.log(
      'Calling createFunc API with:',
      this.selectedId,
      this.selectedFunctionNames
    );

    this.apiService
      .createFunc(this.selectedId, this.selectedFunctionNames)
      .subscribe({
        next: (result) => {
          console.log('API Response:', result);
          // this.toastMessage.success('Selected functions deployed successfully');
          this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Selected functions deployed successfully',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
          this.loder.stop();
        },

        error: (error) => {
          console.error('API Error:', error);
          this.toastMessage.error('Connection failed while deploying');
          this.loder.stop();
        },
      });
  }

  onSelectedFunction() {
    // this.loder.start();

    if (!this.selectedId) {
      // Show toastr message indicating that a project should be selected
      if (!this.isNotificationVisible) {
        this.toastMessage.warning('Please select the project');
        this.isNotificationVisible = true;
        // Hide the notification after a certain time
        setTimeout(() => {
        this.isNotificationVisible = false;
        }, 3000); // Adjust the duration as needed
      }
      return; // Stop further execution since project ID is not selected
    }
    this.selectedFunctionNames = this.Functionlist.filter(
      (option: { isChecked: any }) => option.isChecked
    ).map((option: { functionName: any }) => option.functionName);
    console.log('Selected Function Names:', this.selectedFunctionNames);
    if (this.selectedFunctionNames.length === 0) {
      console.error('No functions selected for deployment.');
      if (!this.isNotificationVisible) {

        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> No functions are selected for deployment',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        // this.toastMessage.warning('No functions are selected for deployment');
        this.isNotificationVisible = true;
        // Hide the notification after a certain time
        setTimeout(() => {
        this.isNotificationVisible = false;
        }, 3000); // Adjust the duration as needed
      }

      this.loder.stop();
      // You can show an error message to the user here if needed
      return;
    }

    this.submitFunctionData();
    // Perform your deploy action here with the selected function names
    // For example, call an API to deploy the selected functions
  }
  Function:any
  Getfunction() {
    this.apiService.getFunctionList().subscribe({
      next: (result) => {
        this.Function = result;
        console.log('function list', this.Function);
      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }
  submitDataBasetype(data: any) {
    this.loder.start();
    this.apiService.getFunctionData(data).subscribe({
      next: (result) => {
        this.Functionlist = result;

        this.Functionlist.forEach((Item1: any) => {
          this.Function.forEach((Item2: any) => {
            if (Item1.functionName === Item2.names) {
              Item1.newColumnName = Item2.functionName;
            }
          });
        });
        this.loder.stop();
        // let temp = [];
        // temp = this.Functionlist.split(';');
        // console.log(temp);
        // this.Functionlist.forEach((item: any) => {
        //   if (item.functionDefinition) {
        //     item.functionDefinition = item.functionDefinition.split(';');
        //   }
        // });
      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }

  CheckedFunction(checkedfunction: any) {
    // this.Functionlist.forEach(
    //   (option: { isChecked: any }) => (option.isChecked = checkedfunction)
    // );
    if (this.colunmnNames.includes(checkedfunction)) {
      // Remove data.columnName from the array if already present
      const index = this.colunmnNames.indexOf(checkedfunction);
      this.colunmnNames.splice(index, 1);
    } else {
      // Add data.columnName to the array if not already present
      this.colunmnNames.push(checkedfunction);
    }
    this.checkArrayLengthAndToggle();
    console.log('Checked Items', this.colunmnNames);
  }

  selectAllFunctions(): void {
    // this.Functionlist = [];
    console.log(this.Functionlist);

    this.Functionlist.forEach((option: { isChecked: boolean, newColumnName: string }) => {
      option.isChecked = this.selectAllChecked;
      console.log(this.selectAllChecked);

      if (this.selectAllChecked && !this.colunmnNames.includes(option.newColumnName)) {
        this.colunmnNames.push(option.newColumnName);
      } else if (!this.selectAllChecked && this.colunmnNames.includes(option.newColumnName)) {
        const index = this.colunmnNames.indexOf(option.newColumnName);
        this.colunmnNames.splice(index, 1);
      }
    });
    this.checkArrayLengthAndToggle();
    console.log('Sammed', this.colunmnNames);
  }

  public checkArrayLengthAndToggle(): void {
    this.isColumnNamesEmpty = this.colunmnNames.length === 0;
    console.log(this.isColumnNamesEmpty);
  }
}
