import { Component, OnInit, TemplateRef, ViewEncapsulation, EventEmitter } from '@angular/core';
import {  ToasterService } from 'angular2-toaster';
import { Patient } from 'models/patient.model';
import { User } from 'models';
import { UploaderOptions, UploadFile, UploadInput, humanizeBytes, UploadOutput } from 'ngx-uploader';
import { environment } from '../../../environments/environment'
import { ReferralHttpService } from 'providers';


@Component({
  selector: 'app-referral',
  templateUrl: './referral.component.html',
  styleUrls: ['./referral.component.scss',
    './../../../scss/vendors/bs-datepicker/bs-datepicker.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class ReferralComponent implements OnInit {
  newPatient: Patient = new Patient();
  newUser: User = new User();
  minDate = new Date(1930, 1, 1);
  maxDate = new Date();
  steps: string[] = ["intro", "caregiver", "patient", "documents"];
  currentstep = "intro";
  public submitted: boolean;
  formData: FormData;
  files: UploadFile[];
  fileNames: string[] ;
  uploadInput: EventEmitter<UploadInput>;
  humanizeBytes: Function;
  dragOver: boolean;
  options: UploaderOptions;
  
  constructor(
    private referralHttpService: ReferralHttpService,
    private toasterService: ToasterService) {
      this.options = { concurrency: 1, maxUploads: 30 };
      this.files = []; // local uploading files array
      this.uploadInput = new EventEmitter<UploadInput>(); // input events, we use this to emit data to ngx-uploader
      this.humanizeBytes = humanizeBytes;
  }

  ngOnInit(): void {
    this.fileNames = [];
    this.newUser = this.newUser.createNew({name :'user', id: 0, email: 'user@email.com' });
    this.newPatient = this.newPatient.createNew({name :'user', id: 0, language: 1, gender: 1, dateOfBirth: new Date(), remarks: "no remarks" , nihii: 199392});
  }

  changeStep(newStep: string):void{
    this.currentstep = newStep;
  }

  addReferral() {
    this.submitted = true; // set form submit to true
    this.referralHttpService.add({patient: this.newPatient, user: this.newUser , fileNames: this.fileNames}).subscribe(data => {
      this.toasterService.pop('success', 'Success', `Referral submitted`);
      this.fileNames =[];
    })
  }

  onUploadOutput(output: UploadOutput): void {
    switch (output.type) {
      case 'allAddedToQueue':
          // uncomment this if you want to auto upload files when added
          // const event: UploadInput = {
          //   type: 'uploadAll',
          //   url: '/upload',
          //   method: 'POST',
          //   data: { foo: 'bar' }
          // };
          // this.uploadInput.emit(event);
        break;
      case 'addedToQueue':
        if (typeof output.file !== 'undefined') {
          this.files.push(output.file);
        }
        break;
      case 'uploading':
        if (typeof output.file !== 'undefined') {
          // update current data in files array for uploading file
          const index = this.files.findIndex((file) => typeof output.file !== 'undefined' && file.id === output.file.id);
          this.files[index] = output.file;
        }
        break;
      case 'removed':
        // remove file from array when removed
        this.files = this.files.filter((file: UploadFile) => file !== output.file);
        break;
      case 'dragOver':
        this.dragOver = true;
        break;
      case 'dragOut':
      case 'drop':
        this.dragOver = false;
        break;
      case 'done':
      this.files.forEach(f => {
        this.fileNames.push(f.name);
      });
      this.addReferral();
      this.currentstep = 'final';
      this.files =[];
      this.fileNames =[];

      // The file is downloaded
        break;
    }
  }
 
  startUpload(): void {
    const event: UploadInput = {
      type: 'uploadAll',
      url: `${environment.baseUrl}/Image/${this.newPatient.nihii}`,
      method: 'POST',
      data: { patient: JSON.stringify(this.newPatient), user: JSON.stringify(this.newUser) }
    };
 
    this.uploadInput.emit(event);
  }
 
  cancelUpload(id: string): void {
    this.uploadInput.emit({ type: 'cancel', id: id });
  }
 
  removeFile(id: string): void {
    this.uploadInput.emit({ type: 'remove', id: id });
  }
 
  removeAllFiles(): void {
    this.uploadInput.emit({ type: 'removeAll' });
  }
}