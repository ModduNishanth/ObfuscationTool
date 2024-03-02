import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  inputData: string ='';
  obfuscatedData: string ='';

  constructor(private toastMessage :ToastrService) {


  }

  ngOnInit(): void {
    // this.obfuscateData()
  }

  obfuscateData() {
    if(this.inputData ==""){
         this.toastMessage.warning('<a><i class="fa-solid fa-triangle-exclamation"></i></a> Please enter your data',
    '',
    { timeOut: 2500, toastClass: 'single-line-toast', enableHtml: true }
  );
    }
    this.obfuscatedData = this.obfuscate(this.inputData);
    if(  this.obfuscatedData !=null){
      return true
    }
    else{
      return false
    }
  }

  private generateRandomString(length: number): string {
    const characters = 'abcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
      const randomIndex = Math.floor(Math.random() * characters.length);
      result += characters.charAt(randomIndex);
    }
    return result;
  }

  private obfuscate(data: string): string {
    const charMap: { [key: string]: string } = {
      'a': '@', 'm': '=', 't': '7', 'f': '^', 'i': '!',
      'q': '*', 'b': '+', 's': '$', 'r': '>', 'e': '3', 'd': '_',
      'w': '%', 'z': '?', 'o': '0'
    };

    data = data.toLowerCase();
    let obfuscatedData = '';
    for (let i = 0; i < data.length; i++) {
      const character = data[i];
      if (charMap.hasOwnProperty(character)) {
        obfuscatedData += charMap[character];
      } else {
        // If character is not in charMap, replace with a random character
        obfuscatedData += this.generateRandomString(1);
      }
    }

    return obfuscatedData;
  }


}
