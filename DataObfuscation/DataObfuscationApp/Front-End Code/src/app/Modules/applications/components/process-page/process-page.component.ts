import { Component, OnInit, Pipe, PipeTransform } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ServiceApiService } from '../../../../service-api.service';
import { ToastrService } from 'ngx-toastr';
import { log } from 'console';
import { NgxUiLoaderService } from 'ngx-ui-loader';

@Pipe({
  name: 'filteredList',
})
export class FilteredListPipe implements PipeTransform {
  transform(items: any[], searchText: string): any[] {
    if (!items) return [];
    if (!searchText) return items;

    searchText = searchText.toLowerCase();
    return items.filter((item) => item.toLowerCase().includes(searchText));
  }
}
@Component({
  selector: 'app-process-page',
  templateUrl: './process-page.component.html',
  styleUrls: ['./process-page.component.css'],
})
export class ProcessPageComponent implements OnInit {

  public ConnectionForm: FormGroup = new FormGroup({});

  status: any;
  data: any;
  Map: any;
  myForm: FormGroup = new FormGroup({});
  selectedFile: File | null = null;
  newTableList: any;
  newcolumnList: any;
  SelectedTable: any;
  SelectedProject: any;
  ProjectList: any;
  selectedProjectvalue: any;
  selectedColumnvalue: any;
  functionValue: any;
  selectedtablevalue: any;
  previewDataa: any;
  FinalPreviewData: any;
  newKeyword: any;
  mappingColumns: any = [];
  isChecked: boolean = true;
  Datatype = [{ value: 'MYSQL' }, { value: 'SQLSERVER' }, { value: 'ORACLE' }];
  selectedItems: any[] = [];
  checkedValues: any[] = [];
  selectedValues: any = [];

  tableList: any = [{ value: 'Student' }, { value: 'Employee' }];
  functionName: any;
  convertedFunctionList: any = [];
  syncData: any;
  checkboxes = document.getElementsByClassName('checkbox');
  keywordList: any = [];
  matchedValues: any;
  checkedItems: any;
  i: any;
  searchText: string = ''; // Search text entered by the user
  searchTerm: any;
  checkedColumnList: string[] = [];
  checkedFunctionList: any = [];
  selectedOption: any = [];
  checkedFunctionIndex: any = [];
  Mapped: any = [];
  MappedData: any = {};
  countN = 1;
  selectedProjectvaluess: any;
  selectedFunctinvalue: { id: number; name: string }[] = [];
  mapStatus: any;
  options: any;
  formGroupArray: any = [];
  page :number =1;
  checkedColumn: any;
  fn: any;
  stagingMapping: any;
  dataStatus: any;
  temp: any = [];
  isNotificationVisible: boolean = false;
  isNotificationVisibleSubmit: boolean = false;
  isNotificationVisibleKeyword: boolean = false;
  isNotificationVisibleExport: boolean = false;
  private _snackBar: any;
  filteredColumnList: any[] = [];


  constructor(
    private apiService: ServiceApiService,
    private _formBuilder: FormBuilder,
    private loder: NgxUiLoaderService,
    private toastMessage: ToastrService
  ) {
    this.myForm = new FormGroup({
      listItems: new FormArray([]),

    });
  }



  get listItems() {
    return this.myForm.get('listItems') as FormArray;
  }

  ngOnInit(): void {

    this.initiateConnectionForm();
    this.CallProjectList();
    this.Getfunction();

  }

  public initiateConnectionForm() {
    this.ConnectionForm = this._formBuilder.group({
      tablelist: ['', Validators.required],
      selectedValue: ['', [Validators.required]],
      constantValue: [''],
    });
  }

  // Getting Connection to Database Code
  submitConnectionForm(data: any) {
    this.apiService.getConnection(data).subscribe({
      next: (result) => {
        this.data = result;
        this.toastMessage.success('Success', 'Connected');
        this.ConnectionForm.reset();
      },
      error: (error) => {
        this.toastMessage.error(error.error);
      },
    });
  }
  onSelected(data: any) {
    this.selectedProjectvalue = this.ProjectList.filter(
      (value: any) => value.projectId === parseInt(data)
    );
    this.getKeyword();
    this.CallTable();

  }

  // Getting and Implementation of Table List code....
  CallTable() {
    this.loder.start();
    this.apiService
      .getTableList(this.selectedProjectvalue[0].projectId)
      .subscribe({
        next: (result) => {
          this.newTableList = result;
          this.CallColumn();
          this.loder.stop();
        },
        error: () => {
          console.log('Invalid Request');
          this.newTableList = null;
        },
      });
  }

  //Column List Code.....
  getTableName: any;
  CallColumn() {
    this.apiService
      .getColumnList(
        this.selectedProjectvalue[0].projectId,
        this.selectedtablevalue
      )
      .subscribe({
        next: (result) => {
          this.newcolumnList = result;
          console.log(this.newcolumnList);
          this.formGroupArray = [];
          Object.keys(this.ConnectionForm.controls).forEach(controlName => {
            this.ConnectionForm.removeControl(controlName);
          });
          const formControlConfig = {};

          this.newcolumnList.forEach((item: any, index: any) => {
            const controlName = 'select_' + item.columnName;
            const selectedValue = item.functionId;
            if (selectedValue === 0) {
              setTimeout(() => {
                this.ConnectionForm.get(controlName)?.setValue(0);
              });
            }
            this.ConnectionForm.addControl(
              controlName,
              new FormControl(selectedValue)
            );
          });

          console.log(this.newcolumnList);
        },
        error: () => {
          console.log('Invalid Request');
        },
      });
  }

  //Search column value is less than 3 characters
  async onSearchInput(e: any) {
    this.page=1
    const searchValue = e?.toLowerCase()?.trim();

    if (searchValue.length >= 3) {
      await this.apiService
        .getColumnList(
          this.selectedProjectvalue[0].projectId,
          this.selectedtablevalue
        )
        .subscribe({
          next: (result: any) => {

            this.newcolumnList = result;
            console.log(this.newcolumnList);

            this.newcolumnList = this.newcolumnList?.filter(
              (item: any) =>
                item.columnName.toLowerCase().indexOf(searchValue) !== -1
            );
            console.log(this.newcolumnList);
          },
          error: () => {
            console.log('Invalid Request', 5656);
          },
        });
    } else {
      // If the search value is less than 3 characters, show the full list
      await this.apiService
        .getColumnList(
          this.selectedProjectvalue[0].projectId,
          this.selectedtablevalue
        )
        .subscribe({
          next: (result: any) => {
            this.newcolumnList = result;
            console.log(this.newcolumnList);
          },
          error: () => {
            console.log('Invalid Request', 5656);
          },
        });
    }
  }


  // Select and Upload File Code.....
  onSubmit() {
    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('file', this.selectedFile);
      this.apiService.uploadFile(formData).subscribe(
        (response) => {
          console.log('File uploaded successfully:', response);
          this.runFile();
        },
        (error) => {
          console.error('Error occurred while uploading the file:', error);
        }
      );
    }
  }

  // Function Creation
  runFile() {
    let ProjectParam = this.selectedProjectvalue;

    this.apiService.runFile(ProjectParam).subscribe(
      (response) => {
        console.log('Successfully Function Created', response);
      },
      (error) => {
        console.error('Error occurred while Running the file:', error);
      }
    );
  }

  // Getting Function List.....
  Functionlist: any;
  Getfunction() {
    this.apiService.getFunctionList().subscribe({
      next: (result) => {
        this.Functionlist = result;
        console.log('function list', this.Functionlist);

        // Convert Functionlist object to array
        this.convertedFunctionList = [];
        for (let key in this.Functionlist) {
          if (this.Functionlist.hasOwnProperty(key)) {
            this.convertedFunctionList.push(this.Functionlist[key]);
          }
        }
      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }

  // Getting Project List Code......
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
  noRecord: boolean = true;
  showInp: boolean = false;
  onSelectedTable(data: any) {
    this.selectedtablevalue = data;
    console.log('Columnvalue', this.selectedtablevalue);
    this.checkedColumnList=["","","","",""]

    this.CallColumn();
    this.noRecord = !this.selectedtablevalue;
    this.showInp = this.selectedtablevalue;
  }

  // Preview functionality implementation
  Preview(data: any) {
    let constantValue = '';
    constantValue = data.constantValue
    // for (const item of this.temp) {
    //   constantValue = item.constantValue;
    // }
    let ProjectParam = this.selectedProjectvalue[0].projectId; //id
    let Tableparam = this.selectedtablevalue; //table
    let FunctionParam = this.Functionlist.find((obj:any) => obj.id == data.functionId); //functionName
    // console.log(FunctionParam.name);

    //let ColumnParam = this.checkedColumnList[0]; //column
    let lastIndex = this.checkedColumnList.length - 1;
    let ColumnParam = data.columnName;
    let dataType = this.selectedProjectvalue[0].datatype;
    this.FinalPreviewData =[];
    //datatype
    this.apiService
      .GetPreview(
        ProjectParam,
        Tableparam,
        FunctionParam.name,
        ColumnParam,
        dataType,
        constantValue
      )
      .subscribe({
        next: (result) => {
          this.previewDataa = result;
          console.log(' Preview Obfuscation Data : ', this.previewDataa);
          this.viewPreviewData(this.previewDataa[0]);
          // console.log(tableValue)
        },
        error: () => {
          console.log('Invalid Request');
        },
      });
  }

  // Taking data for preview and passing value
  onPreviewSubmit(data: any) {
    console.log("input",data);
    this.Preview(data);
    console.log('OnPreviewClick', data.columnName);
  }

  onKeywordPreviewSubmit(columnName: string, constantValue: string, functionId: number, isSelected: number) {
    const data = { columnName, constantValue, functionId, isSelected };
    console.log("input", data);
    this.Preview(data);
    console.log('OnPreviewClick', data.columnName);
  }

  // Getting previw data
  viewPreviewData(data: any) {
    let ProjectParam = this.selectedProjectvalue[0].projectId;
    this.apiService.getPreviewData(data, ProjectParam).subscribe({
      next: (result) => {
        var value :any = result;
        var getResult : any[]=[];
        value.map((res:any)=>{
          getResult.push({
            MaskedData : res.split(","),
            result : res
          })
        });
        this.FinalPreviewData =  getResult;
        console.log(' Obfuscation Data : ', this.FinalPreviewData);
        // console.log(tableValue)
      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }


  clearModelData(){
   this.FinalPreviewData=['','','','','']
  }
  // Getting Keywords
  getKeyword() {
    console.log(
      'Keyword Project ID Test',
      this.selectedProjectvalue[0].projectId
    );

    this.apiService
      .getKeywordList(this.selectedProjectvalue[0].projectId)
      .subscribe({
        next: (result: any) => {
          this.keywordList = result;
          console.log(' Keyword List : ', this.keywordList);
          // console.log(tableValue)
        },
        error: () => {
          console.log('Invalid Request');
        },
      });
  }

  // Submit button code
  getMappingTable() {
    if ( this.selectedProjectvalue == undefined) {
      if (!this.isNotificationVisibleSubmit) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the Project.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisibleSubmit = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleSubmit = false;
        }, 3000); // Adjust the duration as needed
      }
      // this.toastMessage.warning('Please select the required fields');
    }
    else if (
      this.selectedtablevalue == undefined ||
      this.selectedProjectvalue[0].projectId == undefined
    ) {
      if (!this.isNotificationVisibleSubmit) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the table.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisibleSubmit = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleSubmit = false;
        }, 3000); // Adjust the duration as needed
      }
      // this.toastMessage.warning('Please select the required fields');
    }

    else if (
      this.selectedtablevalue == undefined ||

      this.newcolumnList.length == 0 ||
      this.selectedProjectvalue[0].projectId == undefined
    ) {
      if (!this.isNotificationVisibleSubmit) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please Fetch the columns.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisibleSubmit = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleSubmit = false;
        }, 3000); // Adjust the duration as needed
      }
    }
    else if (
      this.selectedtablevalue == undefined ||
      this.temp.length == 0 ||
      this.selectedProjectvalue[0].projectId == undefined
    ) {
      if (!this.isNotificationVisibleSubmit) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the column and obfuscation type.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisibleSubmit = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleSubmit = false;
        }, 3000); // Adjust the duration as needed
      }
    }
    else {
      let hasInvalidItems = this.temp.some((item: { isSelected: number; functionId: number; }) => item.isSelected === 1 && item.functionId === 0);
      if (hasInvalidItems) {
        if (!this.isNotificationVisibleSubmit){
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the obfuscation type.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisibleSubmit = true;
        setTimeout(() => {
          this.isNotificationVisibleSubmit = false;
        }, 3000);
      }
    }
    else {
      let columnName = [];
      let isSelected = [];
      let functionId = [];
      let constantValue = [];
      let certificateName: any[] = []
      for (const item of this.temp) {
        columnName.push(item.columnName);
        isSelected.push(item.isSelected)
        certificateName.push(item.certificateName);
        // functionId.push(item.functionId);
         if (item.functionId !== undefined && item.functionId !== null) {
          functionId.push(item.functionId);
    } else if (item.functionId !== undefined && item.functionId !== null) {
      functionId.push(item.functionId);
    }
        constantValue.push(item.constantValue);
      }
      let tableName = this.selectedtablevalue;
      let projectNo = this.selectedProjectvalue[0].projectId;
      let columnStatus = columnName.map((columnName) => {
        let selectedColumn = this.newcolumnList.find(
          (item: { columnName: string }) => item.columnName === columnName
        );
        return selectedColumn !== undefined && selectedColumn.dataType !== null
          ? selectedColumn.dataType
          : 'Existing';
      });
      this.loder.start();
      this.apiService
        .mappingtable(
          tableName,
          functionId,
          columnName,
          isSelected,
          projectNo,
          columnStatus,
          constantValue,
          certificateName
        )
        .subscribe({
          next: (result: any) => {
            if (result == true) {

              this.loder.stop();
              this.MappedData = {};
             this.CallColumn();
              setTimeout(() => {
                this.Functionlist();
              }, 1000);
              this.toastMessage.success('Map successfully');
              this.temp = [];
            } else {
              this.toastMessage.error('error while fetching data');
              this.loder.stop();
            }
          },
          error: () => {
            console.log('Invalid request');
          },
        });
      }
    }
  }


  // CheckedItems(event: any) {
  //   const columnName: string = event.target.value; // Assuming value is the column name
  //   const selectedColumn = this.newcolumnList.find((column: { columnName: any; }) => column.columnName === columnName);
  //   if (event.target.checked) {
  //     if (!this.checkedColumnList.includes(columnName)) {
  //       this.checkedColumnList.push(columnName);
  //       console.log(this.newcolumnList)

  //       if (selectedColumn.functionId > 0 ) {
  //         let constantValue = this.ConnectionForm.value.constantValue || '';
  //         let certificateName = this.ConnectionForm.value.constantValue || '';
  //         let obj = {
  //           columnName: columnName,
  //           isSelected: 1,
  //           functionId: selectedColumn.functionId,
  //           constantValue: constantValue,
  //           certificateName: certificateName
  //         };
  //         const existingIndex = this.temp.findIndex((obj: any) => obj.columnName === columnName);
  //         if (existingIndex !== -1) {
  //           this.temp.splice(existingIndex, 1);
  //         }
  //         this.temp.push(obj);
  //         this.ConnectionForm.get('constantValue')?.setValue('')
  //       }
  //     }
  //   } else {
  //     this.checkedColumnList = this.checkedColumnList.filter(
  //       (value) => value !== columnName
  //     );

  //     const selectedColumn = this.newcolumnList.find((column: { columnName: any; }) => column.columnName === columnName);

  //     let constantValue = this.ConnectionForm.value.constantValue || '';
  //     let certificateName =this.ConnectionForm.value.constantValue || '';
  //     let obj = {
  //       columnName: columnName,
  //       isSelected: 0,
  //       functionId: selectedColumn.functionId,
  //       constantValue: constantValue,
  //       certificateName: certificateName
  //     }
  //     const existingIndex = this.temp.findIndex((obj: any) => obj.columnName === columnName);
  //     if (existingIndex !== -1) {
  //       this.temp.splice(existingIndex, 1);
  //     }
  //     this.temp.push(obj);
  //     this.ConnectionForm.get('constantValue')?.setValue('')

  //   }
  // }
  CheckedItems(event: any) {
    const columnName: string = event.target.value; // Assuming value is the column name
    const selectedColumn = this.newcolumnList.find((column: { columnName: any; }) => column.columnName === columnName);

    if (event.target.checked) {
      if (!this.checkedColumnList.includes(columnName)) {
        this.checkedColumnList.push(columnName);
        console.log(this.newcolumnList)

        let constantValue = this.ConnectionForm.value.constantValue || '';
        let certificateName = this.ConnectionForm.value.constantValue || '';
        let functionId = selectedColumn.functionId > 0 ? selectedColumn.functionId : 0; // Check if functionId is greater than 0, otherwise set to 0

        let obj = {
          columnName: columnName,
          isSelected: 1,
          functionId: functionId,
          constantValue: constantValue,
          certificateName: certificateName
        };
        this.newcolumnList.forEach((obj : any) => {
          if (this.checkedColumnList.includes(obj.columnName)) {
            obj.isSelected = 1;
          }
        });

        const existingIndex = this.temp.findIndex((obj: any) => obj.columnName === columnName);
        if (existingIndex !== -1) {
          this.temp.splice(existingIndex, 1);
        }
        this.temp.push(obj);
        this.ConnectionForm.get('constantValue')?.setValue('');
      }
    } else {
      this.checkedColumnList = this.checkedColumnList.filter((value) => value !== columnName);

      let constantValue = this.ConnectionForm.value.constantValue || '';
      let certificateName = this.ConnectionForm.value.constantValue || '';
      let functionId = selectedColumn.functionId > 0 ? selectedColumn.functionId : 0; // Check if functionId is greater than 0, otherwise set to 0

      let obj = {
        columnName: columnName,
        isSelected: 0,
        functionId: functionId,
        constantValue: constantValue,
        certificateName: certificateName
      }

      const existingIndex = this.temp.findIndex((obj: any) => obj.columnName === columnName);
      if (existingIndex !== -1) {
        this.temp.splice(existingIndex, 1);
      }
      this.temp.push(obj);
      this.ConnectionForm.get('constantValue')?.setValue('');
    }
  }


  columnStates: { [key: string]: boolean } = {};
  onSelectedFunction(event: any, columnName: string, user: any) {
    console.log(user, columnName);
    const selectedValue = event.target.value;
    console.log(selectedValue,this.newcolumnList);

    if (parseInt(selectedValue) === 0) {
      const zeroFunctionObj = {
        columnName: columnName,
        isSelected: 1,
        functionId: 0,
        constantValue: '',
        certificateName: ''
      };
      this.temp.push(zeroFunctionObj);
    }

    this.ConnectionForm.get('constantValue')?.setValue('');

    // const selectedFunction = selectedValue === '0' ? { id: 0 } : this.Functionlist.find(
    //   (func: { id: number }) => func.id === parseInt(selectedValue)
    // );
    const selectedFunction = this.Functionlist.find(
      (func: { id: number }) => func.id === parseInt(selectedValue)
    );

    this.newcolumnList.forEach((obj : any) => {
      if (this.checkedColumnList.includes(obj.columnName)) {
        obj.isSelected = 1;
      }
    });

    selectedFunction['columnName']=columnName;
    selectedFunction['isSelected'] = this.newcolumnList.find((obj: any) => obj.columnName === columnName)?.isSelected;
    // console.log(selectedFunction);
    this.newcolumnList = this.newcolumnList.map((obj : any) => {
      if (obj.columnName === selectedFunction.columnName) {
        return { ...obj, functionId: selectedFunction.id ,isSelected: selectedFunction.isSelected};
      }
      return obj
    });
    console.log(selectedFunction);



    console.log(this.newcolumnList);


if (selectedFunction.id === 8 || selectedFunction.id ===9) {
      this.columnStates[columnName] = true; // Enable the input for this column
    } else {
      this.columnStates[columnName] = false; // Disable the input for this column
    }

    let constantValue = this.ConnectionForm.value.constantValue || '';
    let certificateName =this.ConnectionForm.value.constantValue || '';
    let obj = {
      columnName: columnName,
      isSelected: selectedFunction.isSelected? 1 : 0,
      functionId: selectedFunction.id,
      constantValue: constantValue,
      certificateName: certificateName
    }


    // Find if the column is already available in array if yes, remove it and then update it with new values.
    const existingIndex = this.temp.findIndex((obj: any) => obj.columnName === columnName);
    if (existingIndex !== -1) {
      this.temp.splice(existingIndex, 1);
    }
    this.temp.push(obj);
    this.ConnectionForm.get('constantValue')?.setValue('')
  }

//onblur method
// assignContantValue(columnName:any)
//   {
//     const index = this.temp.findIndex((x: any) => x.columnName === columnName);
//   if (index !== -1) {
//     this.temp[index].constantValue = this.ConnectionForm.value.constantValue ||'';
//     console.log(this.temp);
//     this.ConnectionForm.get('constantValue')?.setValue('')
//   } else {
//     console.log("Object not found in the array");
//   }
//     // find the object in array using columnname and assign the const value to it and update the object to same array.

//     console.log(this.ConnectionForm.value.constantValue);
//      console.log(this.temp)
//   }

// public onConstantValueChanged(event: any, user: any) {

//   const constValue = event.target.value;
//   this.assignConstantValue(constValue, user.columnName);


// }

public onConstantValueChanged(event: any, user: any) {
  const constValue = event.target.value;

  if (!constValue.trim()) {
    // Display a toast message indicating that constValue is required
    this.toastMessage.warning('Input field is required', '', {
       // Adjust the duration as needed
    });
    return; // Exit the function if constValue is empty
  }

  this.assignConstantValue(constValue, user.columnName);
}

  assignConstantValue(constValue: any, columnName: any) {
    const matchingObject = this.temp.find((obj: any) => obj.columnName === columnName);

    if (matchingObject) {
      const indexOfMatchingObject = this.temp.findIndex((obj: any) => obj.columnName === columnName);
      this.temp[indexOfMatchingObject].constantValue = constValue;

      // Check if functionId is 9 and update certificateName
      if (this.temp[indexOfMatchingObject].functionId === 9) {
        this.temp[indexOfMatchingObject].certificateName = constValue;
      }
    } else {
      let index = this.temp.length - 1;
      this.temp[index].constantValue = constValue;

      // Check if functionId is 9 and update certificateName
      if (this.temp[index].functionId === 9) {
        this.temp[index].certificateName = constValue;
      }
    }
      this.newcolumnList = this.newcolumnList.map((obj2: any) => {
    let matchingObj1 = this.temp.find((obj1: any) => obj1.columnName === obj2.columnName);
    if (matchingObj1) {
      return { ...obj2, constantValue: matchingObj1.constantValue };
    } else {
      return obj2;
    }});
  }

// getDisplayedValue(constantValue: string): string {
//   try {
//     const parsedValue = JSON.parse(constantValue);
//     // Extract the values you want to display
//     const certificateName = parsedValue.CertificateName || '';
//     const constantValueProp = parsedValue.ConstantValue || '';
//     return `${certificateName}${constantValueProp}`;
//   } catch (error) {
//     console.error('Error parsing JSON:', error);
//     return ''; // Handle the error as needed
//   }
// }


applyKeyWord() {
  if ( this.selectedProjectvalue == undefined) {
    if (!this.isNotificationVisibleSubmit) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the Project.',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleSubmit = true;
      // Hide the notification after a certain time
      setTimeout(() => {
        this.isNotificationVisibleSubmit = false;
      }, 3000); // Adjust the duration as needed
    }
    // this.toastMessage.warning('Please select the required fields');
   }
    else if ( this.selectedtablevalue == undefined) {
    if (!this.isNotificationVisibleSubmit) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the Table.',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleSubmit = true;
      // Hide the notification after a certain time
      setTimeout(() => {
        this.isNotificationVisibleSubmit = false;
      }, 3000); // Adjust the duration as needed
    }
    // this.toastMessage.warning('Please select the required fields');
   }

   else if (
    this.selectedtablevalue == undefined || this.newcolumnList.length == 0 || this.selectedProjectvalue[0].projectId == undefined) {
     if (!this.isNotificationVisibleSubmit) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please Fetch the columns.',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleSubmit = true;
      // Hide the notification after a certain time
      setTimeout(() => {
        this.isNotificationVisibleSubmit = false;
      }, 3000); // Adjust the duration as needed
      }
     return
    }
   else if(this.keywordList.length ==0){
    if (!this.isNotificationVisibleKeyword) {
      this.toastMessage.warning('Please add relevant keywords before proceeding',
        '',
        { timeOut: 1500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleKeyword = true;
      // Hide the notification after a certain time
      setTimeout(() => {
        this.isNotificationVisibleKeyword = false;

      }, 3000); // Adjust the duration as needed
    }
    return;

  }else{

  const matchesFound = this.newcolumnList.some((columnItem: any) => {
    return this.keywordList.some((keywordItem: any) => {
      // Convert both columnName and keyWords to lowercase for case-insensitive comparison
      return columnItem.columnName.toLowerCase().includes(keywordItem.keyWords.toLowerCase());
    });
  });

  if (matchesFound) {
    if (!this.isNotificationVisibleKeyword) {
      this.toastMessage.success('Match found.',
        '',
        { timeOut: 1500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleKeyword = true;
      // Hide the notification after a certain time
      setTimeout(() => {
        this.isNotificationVisibleKeyword = false;

      }, 3000); // Adjust the duration as needed
    }
  }
  else {
     if (!this.isNotificationVisibleKeyword) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> No match found.',
        '',
        { timeOut: 1500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleKeyword = true;
      // Hide the notification after a certain time
      setTimeout(() => {
        this.isNotificationVisibleKeyword = false;
      }, 3000); // Adjust the duration as needed
    }
  }
  this.newcolumnList.forEach((columnItem: any) => {
    this.keywordList.forEach((keywordItem: any) => {
      if (columnItem.columnName.toLowerCase().includes(keywordItem.keyWords.toLowerCase())) {
        columnItem.isSelected = 1;
        columnItem.functionId = keywordItem.functionId;
        // Call onPreviewSubmit with individual values
        this.onKeywordPreviewSubmit(columnItem.columnName, '', keywordItem.functionId, 1);
        console.log("kwywordnsk",keywordItem.functionId)
        this.temp.push({ ...columnItem });
      }
    });
  });

  this.temp.forEach((columnItem: any) => {
    let obj: any = {};
    obj[`select_${columnItem.columnName}`] = columnItem.functionId;
    this.ConnectionForm.patchValue(obj);
  });
}



    // this.keywordList.forEach((item: any) => {
    //   const controlName = 'select';
    //   const selectedValue = item.functionId;

    // });

}
  //download Excel file functionality implementation
  downloadFileExcel: any;
  downloadFile() {
    if (this.selectedProjectvalue == undefined) {

      if (!this.isNotificationVisibleExport) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the project.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisibleExport = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleExport = false;
        }, 3000); // Adjust the duration as needed
      }


    }
    else {
      this.apiService
        .downloadFile(this.selectedProjectvalue[0].projectId)
        .subscribe((response: Blob) => {
          const downloadLink = document.createElement('a');
          const fileURL = URL.createObjectURL(response);
          downloadLink.href = fileURL;
          downloadLink.download = 'exported_data.xlsx';
          downloadLink.click();
          URL.revokeObjectURL(fileURL);
          if (!this.isNotificationVisibleExport) {
            this.toastMessage.success('Download successfully');
            this.isNotificationVisibleExport = true;
            // Hide the notification after a certain time
            setTimeout(() => {
              this.isNotificationVisibleExport = false;
            }, 3000); // Adjust the duration as needed
          }
        });
    }
  }

  //  Code for Sync Insert staging Mapping
  Sync() {
    if (this.selectedProjectvalue== undefined) {
      if (!this.isNotificationVisible) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the project.',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisible = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisible = false;
        }, 3000); // Adjust the duration as needed
      }

    }else if( this.selectedtablevalue == undefined ||
      this.selectedProjectvalue[0].projectId == undefined){
        if (!this.isNotificationVisible) {
          this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the table.',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
          );
          this.isNotificationVisible = true;
          // Hide the notification after a certain time
          setTimeout(() => {
            this.isNotificationVisible = false;
          }, 3000); // Adjust the duration as needed
    }
  }
    else {
      this.loder.start();
      this.apiService
        .InsertStgMapping(
          this.selectedProjectvalue[0].projectId,
          this.selectedtablevalue
        )
        .subscribe({
          next: (result) => {
            this.stagingMapping = result;

            if (!this.isNotificationVisible) {
              this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Sync successfully done.',
                '',
                { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
              );
              this.isNotificationVisible = true;
              // Hide the notification after a certain time
              setTimeout(() => {
                this.isNotificationVisible = false;
              }, 3000); // Adjust the duration as needed
        }
            this.loder.stop();
            this.stagingMapping.forEach((newItem: any) => {
              const existingColumnIndex = this.newcolumnList.findIndex(
                (columnItem: any) => columnItem.columnName === newItem.columnName
              );
              if (existingColumnIndex === -1) {
                this.newcolumnList.push({
                  columnName: newItem.columnName,
                  dataType: newItem.dataType,
                });
              } else {
                this.newcolumnList[existingColumnIndex].dataType =
                  newItem.dataType;
                // You can update other properties here as needed
              }
            });
            this.stagingMapping.forEach((item: any, index: any) => {
              const controlName = 'select_' + item.columnName;
              // const selectedValue = item.functionId;
              if ((item.dataType = 'New')) {
                this.ConnectionForm.addControl(controlName, new FormControl(0));
              }
              console.log("stagingMapping",this.stagingMapping)

              console.log("newcolumnlistun",this.newcolumnList)
            });

          },
          error: () => {
            console.log('Invalid Request');
          },
        });
    }
  }
}
