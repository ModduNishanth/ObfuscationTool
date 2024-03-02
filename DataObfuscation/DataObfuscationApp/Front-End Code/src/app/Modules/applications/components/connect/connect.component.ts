import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ServiceApiService } from '../../../../service-api.service';
import { ToastrService } from 'ngx-toastr';
import { log } from 'console';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Router } from '@angular/router';

@Component({
  selector: 'app-connect',
  templateUrl: './connect.component.html',
  styleUrls: ['./connect.component.css'],
})
export class ConnectComponent implements OnInit {
  data: any;

  public ConnectionForm: FormGroup = new FormGroup({});
  selectedProject: string = 'defaultValue';
  Projectli: any = [];
  projectForm: any;
  isFormSubmitted: boolean = false;
  updatePj: any;
  serverName: any;
  databaseName: any;
  userName: any;
  userPassword: any;
  selectedDataType: any;
  projectId: any;
  selectedPj: any;
  selectedProjectvalue: any;
  projectdropdownvalues: any = [];
  ProjectliOptions: any = [];
  copyprojectid: any;
  public ProjectList: any = [];
  testConnection: any;
  formdata: any;
  isLoading = false;
  iconColor: string | undefined;
  dataById: any;
  defaultIcon: string = 'default'; // Set default value to "default" (orange)
  isTrue: any;
  deleteById: any;
  copyatributeselectdropdown: any = [];
  deleteByProjectName: any;
  datatype:any
  copyStatus: { [key: string]: string } = {};
  isButtonDisabled: boolean = false;
  page :number =1;

  constructor(
    private apiService: ServiceApiService,
    private _formBuilder: FormBuilder,
    private toastMessage: ToastrService,
    private loder: NgxUiLoaderService,
    private router: Router
  ) {}

  // Validators.pattern(/^[a-zA-Z0-9_&-]*$/)

  ngOnInit(): void {
    this.serverType();
    this.CallProjectList();
    this.ConnectionForm = this._formBuilder.group({
      projectName: new FormControl('', [Validators.required,Validators.maxLength(100),Validators.pattern(/^\S.*$/)]),
      projectDescription: new FormControl('', Validators.maxLength(100)),
      serverName: new FormControl('', [Validators.required,Validators.maxLength(100)]),
      databaseName: new FormControl('', [Validators.required,Validators.maxLength(100)]),
      username: new FormControl('', [Validators.required,Validators.maxLength(100)]),
      password: new FormControl('', [Validators.required,Validators.maxLength(100)]),
      datatype: new FormControl('', [Validators.required,Validators.maxLength(100)])
    });
  }
  isMouseOver = false;


  onMouseEnter() {
    this.isMouseOver = true;
  }

  onMouseLeave() {
    this.isMouseOver = false;
  }
  //getting the datatype
  serverType(){
    this.apiService.getServerType().subscribe({
      next:(result)=>{
        this.datatype =result;
        console.log(this.datatype)
      }
    })
  }
  reloadCurrentRoute() {
    const currentUrl = this.router.url;

    // Navigate to the same route with skipLocationChange
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
    });
  }

  // Getting Connection to Database Code
  submitConnectionForm(data: any) {
    this.loder.start();
    this.apiService.getConnection(data).subscribe({
      next: (result) => {
        this.data = result;
        if (this.data == true) {
          const username = data.projectName;
          // const message = `${username}`
          this.toastMessage.success(`<i class="fa-solid fa-check"></i> ${username} Saved successfully`,
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );

          this.CallProjectList();
          this.ConnectionForm.reset();
           // Manually set the dropdown value back to the placeholder
           const datatypeControl = this.ConnectionForm.get('datatype');
          if (datatypeControl) {
             datatypeControl.setValue('');
          }
          this.loder.stop();
        } else {
          this.toastMessage.warning(`<i class="fa-solid fa-triangle-exclamation"></i> A project with the same details already exists`,
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );

        // this.editClose();

        // // Reset the form model data
        // this.ConnectionForm.reset();

        // // Manually set the dropdown value back to the placeholder
        // const datatypeControl = this.ConnectionForm.get('datatype');
        // if (datatypeControl) {
        //   datatypeControl.setValue('');
        // }
          this.loder.stop();
        }
      },
      error: (error) => {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Failed to Add project. Please try again.',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
        this.loder.stop();
      },
    });
  }

  getcopydata(data: any) {
    this.ProjectliOptions = this.Projectli.filter(
      (x: any) => x.projectId != data.projectId &&  x.testConnection ==1
    );

    this.ProjectliOptions.map((x: any) => {
      x.oldprojectId = data?.projectId;
      console.log(x.oldprojectId);
    });
    console.log(this.ProjectliOptions);
  }

  CallProjectList() {
    this.apiService.getProjectList().subscribe({
      next: (result: any) => {

        // Here we are holding the test connection icon
        result.map((res: any) => {

          res['connection'] = res.testConnection == 1? 'green': res.testConnection == 0 ?
          'orange': res.testConnection == 2 ? 'red': 'orange';
        });

        result.map((res: any) => {
          res['copyAttributeColor'] = 0;
        });
        console.log(result);
        result.sort((a: any, b: any) => b.projectId - a.projectId);
        this.Projectli = result;
        // this.toastMessage.success('Copied Successfully!');
        this.projectdropdownvalues = result;
      },
      error: () => {
       // this.toastMessage.error('Copy Failed!');
      },
    });
  }
  public updateProject(e: any) {
    if (this.projectForm.invalid) {
      this.isFormSubmitted = true;

      if (this.projectForm.controls['projectName'].hasError('required')) {
        alert('Please Enter the Updated Project Name');

        return;
      }

      if (
        this.projectForm.controls['projectDescription'].hasError('required')
      ) {
        alert('Please Enter Project Description');

        return;
      }
    }
  }

  editToggle(data: any) {
    this.dataById = data;
    this.ConnectionForm.patchValue({
      projectId: data.projectId,
      projectName: data.projectName,
      projectDescription: data.projectDescription,
      serverName: data.serverName,
      databaseName: data.databaseName,
      username: data.username,
      password: data.password,
      datatype: data.datatype,
    });
  }

  editProject(data: any) {
    this.apiService.editConnection(this.dataById.projectId, data).subscribe({
      next: (result) => {
        this.data = result;
        if(this.data==true){
          this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Updated successfully ',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
          this.ConnectionForm.reset();
          this.CallProjectList();
        }
        else{
          this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Update failed - Duplicate entry detected',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
          // this.toastMessage.error('Update failed due to Duplicate entry');
          this.CallProjectList();
        }

      },
      error: (error) => {
        this.toastMessage.error(error.error);
      },
    });
  }
  //Each close button Clear the form data.
  editClose() {
    const datatypeControl = this.ConnectionForm.get('datatype');
    if (datatypeControl) {
      this.ConnectionForm.reset();
      // Set the selected value back after resetting
      if (datatypeControl) {
        datatypeControl.setValue('');
      }

      this.CallProjectList();
    }
  }

  //Delete the project API
  deleteToggle(data: any) {
    this.deleteById = data;
    this.deleteByProjectName = data.projectName;
    //console.log(data.projectName);
  }

  onDelete() {
    this.apiService.onDelete(this.deleteById.projectId).subscribe({
      next: (result: any) => {
        if(result==true){
        this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Deleted successfully ',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
        this.CallProjectList();
        const lastPageIndex = Math.ceil(this.Projectli.length % 10);
              if (this.page > lastPageIndex) {
                this.page = lastPageIndex;
              }
            }else{
              this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> The delete was unsuccessful. Please check it is referenced in Mapped Data. ',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
        this.CallProjectList();
            }
      },
      error: (error) => {
        this.toastMessage.error('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Delete failed ',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
      },
    });
  }

  onSelectedProject(data: any, projectName: any) {
    const filteredProjects = this.projectdropdownvalues.filter(
      (x: { projectName: any }) => x.projectName === data.target.value
    );
    if (filteredProjects.length > 0) {
      this.selectedProjectvalue = filteredProjects[0].projectId;
    }
    console.log('Selected Project', this.selectedProjectvalue, projectName);

    this.apiService.copyData(this.selectedProjectvalue, projectName).subscribe({
      next: (result) => {
        this.data = result;
        console.log('Result :', this.data);
      },
      error: (error) => {
        this.toastMessage.error(error.error);
      },
    });
  }

  onSaveChanges(index: any) {

    if(index==undefined){
      this.toastMessage.warning("Please Select Project")
    }
    this.copyStatus = {};
    console.log(index);
    const value = this.ProjectliOptions.filter(
      (x: any) => x.projectId == index
    );
    this.apiService.copyData(value[0].projectId, value[0].oldprojectId)
      .subscribe({
        next: (result) => {
          this.data = result;
          console.log('Result :', this.data);
          this.Projectli = this.Projectli.map((item: any) => {
            if (item.projectId === value[0].oldprojectId) {
              return { ...item, copyAttributeColor: 1 };
            }
            return item;
          });
          console.log(this.Projectli);
          if (result) {
            this.copyStatus[index] = 'success';
            this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Successfully copied ',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
            this.Projectli = this.Projectli.map((item: any) => {
              if (item.projectId === value[0].oldprojectId) {
                return {...item,copyAttributeColor: 'ri-file-copy-2-line success-icon'};
              }
              return item;
            });
          } else {
            this.copyStatus[index] = 'error';
            this.toastMessage.error('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Failed to copy ',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })

            this.Projectli = this.Projectli.map((item: any) => {
              if (item.projectId === value[0].oldprojectId) {
                return {...item,copyAttributeColor: 'ri-file-copy-2-line error-icon'};
              }
              return item;
            });
          }
        },
        error: (error) => {
          this.copyStatus[index] = 'error';
          this.toastMessage.error(error.error);
        },
      });
  }

  onConnect(id: any) {
    this.loder.start();
    this.apiService.testConnection(id).subscribe(
      (result: any) => {
        const data = result.isError;
        const connectedColor = data ? 'green' : 'red';
        // const project = this.Projectli.find(
        //   (res: any, index: any) => id == index + 1
        // );
        const project= this.Projectli.filter(
          (value: any) => value.connection === parseInt(data)
        );

        if (project) {
          project.connection = connectedColor;
          if (connectedColor === 'green') {
            this.toastMessage.success('Connected successfully', '', {
              timeOut: 2500,toastClass: 'single-line-toast', enableHtml: true
            });
            this.CallProjectList();
            this.loder.stop();
          } else {
            this.toastMessage.error(
              '<a><i class="fa-solid fa-triangle-exclamation"></i></a> Connection Failed.',
              '',
              { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
            );
            this.CallProjectList();
            this.loder.stop();
          }
        } else {
          this.loder.stop();
          this.toastMessage.error(
            '<a><i class="fa-solid fa-triangle-exclamation"></i></a>Connection Failed. Please make sure the connection details are correct',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
          );
          this.CallProjectList();
        }
      },
      (error: any) => {
        this.loder.stop();
        this.toastMessage.error(
          '<a><i class="fa-solid fa-triangle-exclamation"></i></a> Failed.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.CallProjectList();
      }

    );

    this.toastMessage.warning(
      '<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please wait we are trying to connect....',
      '',
      { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
    );

    this.isButtonDisabled = true;

    // Enable the button after 3 seconds
    setTimeout(() => {
      this.isButtonDisabled = false;
    }, 3000);

    console.log('this.Projectli', this.Projectli);
  }

  //Password icon
  password: string = '';
  hidePassword: boolean = true;
  togglePasswordVisibility() {
    this.hidePassword = !this.hidePassword;
  }
}
