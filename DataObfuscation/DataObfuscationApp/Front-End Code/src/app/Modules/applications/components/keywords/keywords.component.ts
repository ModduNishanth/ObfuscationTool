import { Component } from '@angular/core';
import { ServiceApiService } from '../../../../service-api.service';
import { ToastrService } from 'ngx-toastr';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';


@Component({
  selector: 'app-keywords',
  templateUrl: './keywords.component.html',
  styleUrls: ['./keywords.component.css']
})
export class KeywordsComponent {
  selectedProjectvalue: any;
  keywordList: any = []
  Fns: any = []
  myForm: FormGroup<{ listItems: FormArray<never>; }>;
  ProjectList: any
  public ConnectionForm: FormGroup = new FormGroup({});
  Functionlist:any
  selectedFunction:any
  selectedKeyword:any
  functionLists:any
  newKeyword:any
  checkValue :any
  updatedEdit:any
  page : number =1;
  newKey: any;
  ngOnInit(): void {
this.initiateConnectionForm()
    this.CallProjectList();
    this.getFunctions()
  }
  constructor(
    private apiService: ServiceApiService,
    private toastMessage: ToastrService,
    private _formBuilder: FormBuilder,
    private loder: NgxUiLoaderService
  ) {
    this.myForm = new FormGroup({
      listItems: new FormArray([])
    });


  }

  public initiateConnectionForm() {
    this.ConnectionForm = this._formBuilder.group({
      tablelist: [''],
       dropdownControl: ['', [Validators.required]],
       formInput:['', [Validators.required] ],
       keywordId:[],

    })
  }
  CallProjectList() {

    this.apiService.getProjectList().subscribe({
      next: (result:any) => {
        this.ProjectList = result.filter((project:any) => project.testConnection === 1);
        console.log(' Project list', this.ProjectList);
        // this.CallTable()

      },
      error: () => {
        console.log("Invalid Request")
      }
    })
  }
//When project is not selected
isNotificationVisible: boolean = false;

notSelectedProject()
{
  // if(this.selectedProjectvalue ==undefined){
  //   this.toastMessage.warning('Please select the project');
  // }

  if (!this.selectedProjectvalue) {
    // Show toastr message indicating that a project should be selected
    if (!this.isNotificationVisible) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the project',
      '',
      { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
      this.isNotificationVisible = true;
      // Hide the notification after a certain time
      setTimeout(() => {
      this.isNotificationVisible = false;
      }, 3000); // Adjust the duration as needed
    }
    return; // Stop further execution since project ID is not selected
  }
}

//Close button in Add Keyword
addKeywordClose(){
  this.ConnectionForm.get('dropdownControl')?.setValue('');
  this.getKeyword();
  this.getFunctions();
}

  // Getting Keywords
  getKeyword() {
    this.ConnectionForm.controls['formInput'].setValue(null)
    this.apiService.getKeywordList(this.selectedProjectvalue[0].projectId).subscribe({
      next: (result: any) => {
        this.keywordList = result;
        result.sort((a: any, b: any) => b.keywordId - a.keywordId);

        this.checkValue = this.selectedProjectvalue[0].projectId
      },
      error: () => {
        console.log("Invalid Request")
      }
    })
  }

  // Adding Keywords
  addKeyword(data: any) {
      // Check if the newKeyword already exists in the keywordList
      const keywordExists = this.keywordList.some((data: any) => data === this.newKeyword);

   this.newKeyword = data;

    if (!this.newKeyword) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the required fields.',
      '',
      { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
    );
      return;
    }

   if (keywordExists) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Keyword already exists in the list.',
      '',
      { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
    );
      return;
    }

    if (!this.selectedProjectvalue[0]?.projectId) {
      this.toastMessage.warning('No selected project');
      return;
    }

    if (!this.selectedFunction) {
      this.toastMessage.warning('No selected function');
      return;
    }

    if (this.newKeyword.length <= 20) {
      this.apiService.addKeywordList(this.selectedProjectvalue[0].projectId, this.newKeyword, this.selectedFunction).subscribe({
        next: (result: any) => {
          this.newKey = result;
          if(this.newKey == true){
            this.keywordList.push(result);
            this.getKeyword();
            this.toastMessage.success('<i class="fa-solid fa-check"></i> Keyword added successfully.',
              '',
              { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
            );
            this.ConnectionForm.get('dropdownControl')?.setValue('');
            this.getFunctions();
          }else{
            this.getKeyword();
            this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a>Keyword already exist.',
              '',
              { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
            );
            this.ConnectionForm.get('dropdownControl')?.setValue('');
            this.getFunctions();
          }

        },
        error: () => {
          console.log("Invalid request");
          this.ConnectionForm.get('dropdownControl')?.setValue('');
          this.getKeyword();
        }
      });
    } else {
      // Show a message or handle the case where the keyword length exceeds 20 characters.
      console.log("Keyword length should be 20 characters or less");
      this.toastMessage.error('<i class="fa-solid fa-check"></i> Keyword length should be 20 characters or less.',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
          );

    }
  }

  
  // Function to reset form fields and other necessary data
  resetFormFields() {
    this.newKeyword = ''; // Reset newKeyword field
    this.ConnectionForm.get('dropdownControl')?.setValue(''); // Reset dropdown selection
    this.getFunctions(); // Refresh other necessary data
  }
  


  onKeywordInput(value: string) {
    this.ConnectionForm.controls['formInput'].setValue(value);
  }

   trimLeadingSpaces(value: string): string {
    let index = 0;

    // Find the index of the first non-space character
    while (index < value.length && value.charAt(index) === ' ') {
      index++;
    }

    // Return the substring starting from the first non-space character
    return value.substring(index);
  }
  trimInputValue(value: string): string {
    return this.trimLeadingSpaces(value);
  }


  areRequiredFieldsValid() {
    return this.ConnectionForm.valid;
  }

  onSelected(data: any) {
    this.loder.start();
    this.selectedProjectvalue = this.ProjectList.filter(
      (value: any) => value.projectId === parseInt(data)
    );
    this.getKeyword()
    this.loder.stop();
  }

  editToggle(data: any) {
    this.ConnectionForm.patchValue({
      formInput:data.keyWords,
      dropdownControl:data.functionId,
      keywordId :data.keywordId
    });

  }


  //Edit Keyword
  editKeyword(data:any){
    this.apiService.editKeywordList(
      this.selectedProjectvalue[0].projectId,
      data.formInput,
      data.keywordId,
      data.dropdownControl
    ).subscribe({
      next: (result: any) => {
        console.log("API Call Successful. Result:", result);
        this.updatedEdit = result;
        if(this.updatedEdit == true){
          this.toastMessage.success('<i class="fa-solid fa-check"></i>Updated successfully.',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
          );
          this.getKeyword();
        }else{
          console.log("Keyword not updated");
        }
          error: (error: any) => {
            console.error("Error occurred:", error);
        }
      },
    });
  }


  // Deleting Keywords
  deleteKeyword(data: any) {
     this.loder.start();
    const newKeyword = data; // Create the item data object
    this.apiService.deleteKeyword(this.selectedProjectvalue[0].projectId,newKeyword).subscribe({
      next: (result: any) => {
        this.toastMessage.success('<i class="fa-solid fa-check"></i> Keyword deleted successfully.',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.loder.stop();
        this.getKeyword();
        const lastPageIndex = Math.ceil(this.keywordList.length % 10);
        if (this.page > lastPageIndex) {
          this.page = lastPageIndex;
        }
      },
      error: () => {
        console.log("Invalid Request")
        this.getKeyword();

      }
    });
  }
  getFunctions() {
    this.apiService.getFunctionList().subscribe({
      next: (result) => {
        this.Functionlist = result;
        console.log('Function list', this.Functionlist);

        // Find the function that matches this.selectedFunction
        const matchedFunction = this.Functionlist.find((item: { someProperty: any; }) => item.someProperty === this.selectedFunction);
        // Replace 'someProperty' with the actual property you want to match

        if (matchedFunction) {
          // Add the matched function to the global array
          this.Fns.push(matchedFunction);
          console.log('Global array of matched functions', this.functionLists);
        } else {
          console.log('No matching function found.');
        }
      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }


  onSelectedFunction(data:any){
    console.log("Function Number",data);
    this.selectedFunction=data

  }
}
