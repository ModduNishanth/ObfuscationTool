import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root',
})
export class ServiceApiService {
  getHighlightedDetails() {
    throw new Error('Method not implemented.');
  }
  private deleteEndpoint = 'KeyWordList/DeleteteKeyword';
  private deleteProject = 'ConnectionString/DeleteProject';
  constructor(private http: HttpClient) {}

  // Get Method API for Connection
  getConnection(data: any) {
    return this.http.post(
      environment.baseUrl + 'ConnectionString/CreateConnectionString',
      data
    );
  }

   //Get server type data
   getServerType(){
    return this.http.get(environment.baseUrl + 'ConnectionString/GetServerTypes')
  }
 
  //For Edit API
  editConnection(projectId: number, formData: any) {
    let urlForEdit =
      environment.baseUrl +
      'ConnectionString/EditConnectionString?projectId=' +
      projectId;
    return this.http.put(urlForEdit, formData);
  }

  downloadFile(projectId: any) {
    let options = {
      projectId: projectId,
    };
    return this.http.post(
      environment.baseUrl + 'MappingTable/api/excel/download',
      options,
      { responseType: 'blob' }
    );
  }
  // Get Method API for Table List
  getTableList(data: any) {
    let options = {
      projectId: data,
    };
    return this.http.post(
      environment.baseUrl + 'GetTableColumnFunction/GetTables',
      options
    );
  }

  // Get Method API for column List

  getColumnList(projectId: any, tableName: any) {
    const options = {
      projectId: projectId,
      tableName: tableName,
    };

    return this.http.post(
      environment.baseUrl + 'GetTableColumnFunction/GetMappedColumnNames',
      options
    );
  }

  // Post Method API for Uploading File
  uploadFile(data: any) {
    return this.http.post(
      environment.baseUrl + 'ConnectionString/upload',
      data,
      { responseType: 'text' }
    );
  }

  runFile(data: any) {
    const headers: HttpHeaders = new HttpHeaders({
      // "_id" : countId,
      'content-type': 'application/json',
    });
    const ProjectName = data;
    let params = new HttpParams();
    params = params.append('projectName', ProjectName);

    // params = params.append("fields", "_all");
    const options = {
      headers: headers,
      params: params,
    };
    return this.http.get(
      environment.baseUrl + 'GetTableColumnFunction/GetFunction',
      options
    );
  }

  getFunctionData(projectId: any) {
    let options = {
      projectId: projectId,
    };
    return this.http.post(
      environment.baseUrl +
        'GetTableColumnFunction/GetObfuscationFunctionQuery',
      options
    );
  }

  // Get Method API for Getting Function List
  getFunctionListt(data: any) {
    return this.http.get(
      environment.baseUrl + 'GetTableColumnFunction/GetObfuscationFunctionQuery'
    );
  }
  getFunctionList() {
    return this.http.get(
      environment.baseUrl + 'GetTableColumnFunction/GetFunctionNames'
    );
  }

  addFunction() {
    return this.http.get(
      environment.baseUrl + 'GetTableColumnFunction/GetViewInfo'
    );
  }
  createFunc(projectId: any, FunctionName: any[]) {
    const options = {
      projectId: projectId,

      functionName: FunctionName,
    };
    return this.http.post(
      environment.baseUrl + 'UpdateFunction/CreateFunctions',
      options
    );
  }
  getPreview(projectName: any, FunctionName: any) {
    const options = {
      projectName: projectName,
      functionName: FunctionName,
    };

    return this.http.post(
      environment.baseUrl + 'UpdateFunction/CreateFunctions',
      options
    );
  }

  // Get Method API for Getting Project List
  getProjectList() {
    return this.http.get(environment.baseUrl + 'ConnectionString/GetProject');
  }

  //Get Preview
  //Get Preview
  GetPreview(
    projectId: any,
    tableName: any,
    functionName: any,
    columnName: any,
    dataType: any,
    constantValue: any
  ) {
    const options = {
      projectId: projectId,
      tableName: tableName,
      functionName: functionName,
      columnName: columnName,
      dataType: dataType,
      ConstantValue: constantValue,
    };

    return this.http.post(
      environment.baseUrl + 'Preview/GetPreviewquery',
      options
    );
  }

  getPreviewData(data: any, projectId: any) {
    const options = {
      projectId: projectId,
      executeSelectQuery: data,
    };
    return this.http.post(
      environment.baseUrl + 'Preview/ViewPreviewQuery',
      options
    );
  }
  //Getting Keywords
  getKeywordList(projectId: any) {
    const options = {
      projectId: projectId,
    };
    return this.http.post(
      environment.baseUrl + 'KeyWordList/GetKeyWordcolumns',
      options
    );
  }
  //Adding Keywords
  addKeywordList(ProjectId: any, data: any[], functionId: any) {
    const options = {
      ProjectId: ProjectId,
      keyWords: data,
      deleteKeyword: 'string',
      functionId: functionId,
    };
    return this.http.post(
      environment.baseUrl + 'KeyWordList/UpdateKeywordList',
      options
    );
  }

   //Edit Keywords
   editKeywordList(projectId: any, editKeyword: any,keywordId:any, functionId: any) {
    const options = {
      projectId: projectId,
      editKeyword: editKeyword,
      functionId: functionId,
      keywordId: keywordId
    };
    return this.http.put(
      environment.baseUrl + 'KeyWordList/EditKeyword',
      options 
    );
  }

   //deleting Keywords

   deleteKeyword(projectId: any, deleteKeyword: string): Observable<any> {
    const url = `${environment.baseUrl}${this.deleteEndpoint}`;
    const data = {
      projectId: projectId,
      keyWords: ' ',
      deleteKeyword: deleteKeyword,
      functionId: 0,
    };

    return this.http.delete(url, {
      headers: new HttpHeaders().set('Content-Type', 'application/json'),
      body: data,
    });
  }
  //Submit Mapping table

  mappingtable(
    tableName: any,
    functionNo: any,
    columnName: any,
    isSelected:any,
    projectID: any,
    columnStatus: any,
    constantValue: any,
    certificateName:any
  ) {
    const options = {
      tableName: tableName,
      functionNo: functionNo,
      columnName: columnName,
      isSelected:isSelected,
      projectID: projectID,
      columnStatus: columnStatus,
      constantValue: constantValue,
      certificateName:certificateName

    };
    return this.http.post(
      environment.baseUrl + 'MappingTable/AddFunctionNo',
      options
    );
  }

  //Last Page
  GetMappedTables(projectId: any) {
    let options = {
      projectId: projectId,
    };
    return this.http.post(
      environment.baseUrl + 'MappingTable/GetMappedTables',
      options
    );
  }

  Obfuscation(projectId: any, data: any, projectName: any) {
    let options = {
      projectId: projectId,
      projectName: projectName,
      tableName: data,
    };
    return this.http.post(
      environment.baseUrl + 'Obfuscation/UpdateObfuscationQuery',
      options
    );
  }

  DeleteTable(projectId:any,data:any ){
    let options={
      projectId:projectId,
      tableName :data
    }
    return this.http.post(
      environment.baseUrl + 'Obfuscation/DeleteTable',
      options
    )
  }

  checkFunctionInfo(projectId: any) {
    let options = {
      projectId: projectId,
    };
    return this.http.post(
      environment.baseUrl + 'Obfuscation/CheckFunctionInfo',
      options
    );
  }
  UpdateObfuscateColumn(projectId: any) {
    let options = {
      projectId: projectId,
    };
    return this.http.post(
      environment.baseUrl + 'Obfuscation/UpdateObfuscateColumn',
      options
    );
  }

  //copy project content

  copyData(projectId: any, projectIdfrom: any) {
    {
      let options = {
        projectId: projectId.toString(),
        projectIdfrom: projectIdfrom.toString(),
      };
      return this.http.post(
        environment.baseUrl + 'UpdateFunction/CopyProject',
        options
      );
    }
  }
  testConnection(projectId: any) {
    return this.http.get(
      environment.baseUrl +
        'ConnectionString/TestConnectionString?projectId=' +
        projectId
    );
  }

  onDelete(projectId: any): Observable<any> {
    const url = `${environment.baseUrl}${this.deleteProject}`;
    const data = {
      projectId: projectId,
    };

    return this.http.post(url, data);
  }

  //Sync
  InsertStgMapping(projectId: any, tableName: any) {
    {
      let options = {
        projectId: projectId,
        tableName: tableName,
        projectName: 'string',
      };
      return this.http.post(
        environment.baseUrl + 'MappingTable/GetColumnName&DataType',
        options
      );
    }
  }
  
}
