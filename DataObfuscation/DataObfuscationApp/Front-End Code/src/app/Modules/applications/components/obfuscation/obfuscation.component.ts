import { Component, OnInit } from '@angular/core';
import { ServiceApiService } from '../../../../service-api.service';
import { FormBuilder } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { NgxUiLoaderService } from 'ngx-ui-loader';

@Component({
  selector: 'app-obfuscation',
  templateUrl: './obfuscation.component.html',
  styleUrls: ['./obfuscation.component.css'],
})
export class ObfuscationComponent implements OnInit {
  updateQueryDisplay: any;
  updatefunction: any;
  functionDefinitin: any;
  functionDefinitionqq: any;
  obfuscationData: any;
  checkBoxList: any = '';
  rawdata: any;
  isAllChecked: boolean = false;
  checkInfo: any;
  updatecoll: any;
  errorMessage: string = '';
  matchedTableName: string = '';
  matchedErrorMessage: any; string = '';
  showMatchedTableInfo!: boolean;
  isProjectSelected: boolean = false;
  resetCheckbox: boolean = false;
  isNotificationVisibleObfuscate: boolean = false;
  isNotificationVisibleInvalid: boolean = false;


  constructor(
    private apiService: ServiceApiService,
    private toastMessage: ToastrService,
    private loder: NgxUiLoaderService,
    private _formBuilder: FormBuilder
  ) { }
  newTableList: any = [];
  selectedtablevalue: any;
  newcolumnList: any;
  ProjectList: any;
  selectedProjectvalue: any;
  page: number = 1
  ngOnInit(): void {
    this.CallProjectList();
    this.matchedErrorMessage
  }

  isChecked: string[] = [];

  CallProjectList() {
    this.apiService.getProjectList().subscribe({
      next: (result: any) => {
        this.ProjectList = result.filter((project: any) => project.testConnection === 1);
      },
      error: () => {
        console.log('Invalid Request');
      },
    });
  }

  CallMappedTable() {
    // this.newTableList = [];
    this.apiService
      .GetMappedTables(this.selectedProjectvalue[0].projectId)
      .subscribe({
        next: (result) => {
          if (result == '' || result == undefined) {
            this.newTableList = [];
          } else {
            this.rawdata = result;
            this.newTableList = [];
            this.rawdata.forEach((res: any) => {
              let temp = {
                name: res,
                isChecked: false,
              };
              this.newTableList.push(temp);
            });
            console.log(this.newTableList);
            console.log('checktable');
          }
        },
        error: () => {
          console.log('Invalid Request');
          this.newTableList = null;
        },
      });
  }

  onSelected(data: any) {
    this.selectedProjectvalue = this.ProjectList.filter(
      (value: any) => value.projectId === parseInt(data)
    );
    this.CallMappedTable();
    this.isProjectSelected = true;
  }
  //checkTrueFalse

  ObfuscationQuery(event: any) {
    if (event.target.checked) {
      if (this.checkBoxList == '' || this.checkBoxList == undefined) {
        this.checkBoxList = event.target.value;
      } else {
        this.checkBoxList = this.checkBoxList + ',' + event.target.value;
      }
    } else {
      this.checkBoxList = this.checkBoxList.replace(event.target.value + ',', '');
      this.checkBoxList = this.checkBoxList.replace(',' + event.target.value, '');
      this.checkBoxList = this.checkBoxList.replace(event.target.value, '');
    }
    this.resetCheckbox = this.newTableList.some((option: any) => option.isChecked);
  }

  // Obfuscation() {
  //   this.apiService
  //     .Obfuscation(
  //       this.selectedProjectvalue[0].projectId,
  //       this.checkBoxList,
  //       this.selectedProjectvalue[0].projectName
  //     )
  //     .subscribe({
  //       next: (result) => {
  //         this.updateQueryDisplay = result;

  //         // this.obfuscation(this.obfuscationData,this.updateQueryDisplay);
  //         console.log('UpdateQuery', this.updateQueryDisplay);
  //         this.newTableList.forEach((res: any) => {
  //           res.isChecked = false;
  //         });
  //         this.isAllChecked = false;
  //         this.checkBoxList = '';
  //       },
  //       error: () => {
  //         console.log('Invalid Request');
  //       },
  //     });
  // }

  Obfuscation() {

    this.apiService

      .Obfuscation(
        this.selectedProjectvalue[0].projectId,

        this.checkBoxList,

        this.selectedProjectvalue[0].projectName
      )

      .subscribe({
        next: (result: any) => {
          this.updateQueryDisplay = result;

          //         // this.obfuscation(this.obfuscationData,this.updateQueryDisplay);
          console.log('UpdateQuery', this.updateQueryDisplay);
          if (result && result.tableName) {
            const tableNames: string[] = this.checkBoxList
              .split(',')
              .map((tableName: string) => tableName.trim());

            if (tableNames.includes(result.tableName)) {
              this.matchedTableName = result.tableName;

              this.matchedErrorMessage = result.errorMessage;
              this.showMatchedTableInfo = true;
              // Set the flag to show the error messag
              this.toastMessage.error('<a><i class="fa-solid fa-triangle-exclamation"> </i></a>' +
                "Table : " + this.matchedTableName + ':  Obfuscation process failed with error',

                '',
                { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
              );
              // this.toastMessage.error(
              //   this.matchedErrorMessage,
              //   this.matchedTableName
              // );

              console.log('Matched table name:', this.matchedTableName);

              console.log('Error message:', this.matchedErrorMessage);
            }
          }
          else {
            this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Obfuscated successfully',
              '',
              { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
          }
          this.newTableList.forEach((res: any) => {
            res.isChecked = false;

            res.errorMessage = null;
          });

          this.isAllChecked = false;

          this.checkBoxList = '';
        },

        error: (error) => {
          console.error('Obfuscation error:', error);

          this.errorMessage = error.message;
        },
      });
  }

  checkUncheckAll(event: any) {
    if (event.target.checked) {
      this.newTableList.forEach((res: any) => {
        if (this.checkBoxList == '' || this.checkBoxList == undefined) {
          this.checkBoxList = res.name.tableNames;
          res.isChecked = true;

        } else {
          this.checkBoxList = this.checkBoxList + ',' + res.name.tableNames;
          res.isChecked = true;
        }
      });
    } else {
      this.checkBoxList = '';
      this.newTableList.forEach((res: any) => {
        res.isChecked = false;
      });
    }
  }

  OnSubmit() {
    if (!this.selectedProjectvalue || this.selectedProjectvalue.length === 0) {

      if (!this.isNotificationVisibleObfuscate) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the project',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
        this.isNotificationVisibleObfuscate = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleObfuscate = false;
        }, 3000); // Adjust the duration as needed
      }
      return; // Exit the function since no project is selected
    }else if (this.newTableList.length === 0) {
      if (!this.isNotificationVisibleObfuscate) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> No table exists in the selected project',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleObfuscate = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleObfuscate = false;
        }, 3000);
        return;
      }
    }

    if (!this.checkBoxList) {
      if (!this.isNotificationVisibleObfuscate) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the table',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
        this.isNotificationVisibleObfuscate = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleObfuscate = false;
        }, 3000); // Adjust the duration as needed
      }
      return;
    }

    this.loder.start();
    this.apiService
      .checkFunctionInfo(this.selectedProjectvalue[0].projectId)
      .subscribe({
        next: (result) => {
          this.checkInfo = result;
          if (this.checkInfo == true) {
            this.Obfuscation();

            // this.newTableList = [];
            setTimeout(() => {
              this.CallMappedTable();
              this.loder.stop();
            }, 3000);


          } else {
            if (!this.isNotificationVisibleInvalid) {
              this.toastMessage.warning('Invalid', 'go to the mapping page and select the function');
              this.isNotificationVisibleInvalid = true;
              // Hide the notification after a certain time
              setTimeout(() => {
                this.isNotificationVisibleInvalid = false;
              }, 3000); // Adjust the duration as needed
            }
            this.toastMessage.warning(
              'Invalid', 'go to the mapping page and select the function'
            );
          }
        },
        error: () => {
          console.log('Invalid Request');
        },
      });
  }
  OnDeleteModel() {
    if (this.selectedProjectvalue === undefined || this.selectedProjectvalue.length === 0) {

      if (!this.isNotificationVisibleObfuscate) {
        if (!this.isNotificationVisibleObfuscate) {
          this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the project',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
          this.isNotificationVisibleObfuscate = true;
          // Hide the notification after a certain time
          setTimeout(() => {
            this.isNotificationVisibleObfuscate = false;
          }, 3000); // Adjust the duration as needed
        }
        // Exit the function since no project is selected
      }
    }
    else if (this.newTableList.length === 0) {
      if (!this.isNotificationVisibleObfuscate) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> No table exists in the selected project',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleObfuscate = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleObfuscate = false;
        }, 3000);
        return;
      }
    }
      else {
        if (!this.checkBoxList) {
          if (!this.isNotificationVisibleObfuscate) {
            this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the Table',
              '',
              { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
            this.isNotificationVisibleObfuscate = true;
            // Hide the notification after a certain time
            setTimeout(() => {
              this.isNotificationVisibleObfuscate = false;
            }, 3000); // Adjust the duration as needed
          }
          return;
        }
      }


  }

  OnDelete() {
    this.loder.start();
    const projectId = this.selectedProjectvalue[0].projectId;
    const tableName = this.checkBoxList;
    this.apiService.DeleteTable(projectId, tableName).subscribe(
      (result) => {
        if (result == true) {
          this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Deleted successfully',
            '',
            { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
          console.log('Table deleted');

          this.CallMappedTable()
          this.checkBoxList = '';
          this.loder.stop()
        } else {
          this.toastMessage.error('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Failed to delete',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
          console.log('Delete Failed')
        }
      }
    )
  }





  UpdateColumn() {
    if (!this.selectedProjectvalue || this.selectedProjectvalue.length === 0) {

      if (!this.isNotificationVisibleObfuscate) {
        this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please select the project',
          '',
          { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
        );
        this.isNotificationVisibleObfuscate = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleObfuscate = false;
        }, 3000); // Adjust the duration as needed
      }
      return; // Exit the function since no project is selected
    } else if (this.newTableList.length === 0) {
      if (!this.isNotificationVisibleObfuscate) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> No table exists in the selected project',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleObfuscate = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleObfuscate = false;
        }, 3000);
        return;
      }
    }
    else if (!this.hasValidTable()) {
      if (!this.isNotificationVisibleObfuscate) {
      this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please obfuscate atleast one table to Re-enable Obfuscation. ',
        '',
        { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
      );
      this.isNotificationVisibleObfuscate = true;
        // Hide the notification after a certain time
        setTimeout(() => {
          this.isNotificationVisibleObfuscate = false;
        }, 3000);
        return;
      }
    }


    else {
      this.loder.start();
      // this.newTableList = [];
      this.apiService
        .UpdateObfuscateColumn(this.selectedProjectvalue[0].projectId)
        .subscribe({
          next: (result) => {
            if(result==true){
              this.updatecoll = result;
              this.toastMessage.success('<a><i class="fa-solid fa-check"></i></a> Reset successfully ',
                '',
                { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
              // this.newTableList = [];
              this.CallMappedTable();
              this.matchedErrorMessage = '';
              this.loder.stop();
            }else {
              this.toastMessage.error('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please obfuscate atleast one table to Re-enable Obfuscation. ',
                '',
                { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true })
                // this.newTableList = [];
                this.CallMappedTable();
                this.loder.stop();
            }

          },
          error: () => {
            console.log('invalid Request');
          },
        });
      this.resetCheckbox = false;
    }
  }
  hasValidTable(): boolean {
    return this.newTableList.some((table: { isChecked: any; name: { obfuscated: any; }; }) => table.isChecked || table.name.obfuscated);
  }
}
