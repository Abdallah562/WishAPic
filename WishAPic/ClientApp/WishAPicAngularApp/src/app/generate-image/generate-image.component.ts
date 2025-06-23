// import { Component } from '@angular/core';
// import { SdxlService } from '../services/sdxl.service';
// import { AccountService } from '../services/account.service';
import Toastify from 'toastify-js';
// @Component({
//   selector: 'app-generate-image',
//   templateUrl: './generate-image.component.html',
//   styleUrl: './generate-image.component.css'
// })
// export class GenerateImageComponent {

//   prompt: string = '';
//   brandName: string = '';
//   brandStyle: string = '';
//   imageUrl: string | null = null;
//   constructor(private sdxlService: SdxlService,
//     private accountService: AccountService) { }

//   generateImage() {
//     this.imageUrl = null;
//     if (!this.prompt || !this.brandName) return;
//     this.sdxlService.generateImage(this.brandName,this.brandStyle,this.prompt).subscribe(response => {
//       this.imageUrl = response;
//     });
//   }

//   refreshClicked(): void {
//     this.accountService.postGenerateNewToken().subscribe({
//       next: (response: any) =>{
//         localStorage["token"] = response.token;
//         localStorage["refreshToken"] = response.refreshToken;

//       },
//       error: (error: any) =>{
//         console.log(error);
//       },
//       complete: () =>{},
//     })
//   }
// }

import { Component } from '@angular/core';
import { SdxlService } from '../services/sdxl.service';
import { ImgData } from '../models/image-data';
import { ImagesService } from '../services/images.service';

@Component({
  selector: 'app-generate-image',
  templateUrl: './generate-image.component.html',
  styleUrls: ['./generate-image.component.css']
})
export class GenerateImageComponent {


  prompt: string = '';
  brandName: string = '';
  brandStyle: string = '';
  // imageUrl: any | null = null;
  isLoading: boolean = false;
  // images: any[] | null = null;
  isModalOpen: boolean = false;
  imagesData: ImgData[] | null = null;
  generatedImage: ImgData | null = null;
  constructor(private sdxlService: SdxlService,private imagesService: ImagesService) {}

  addToFavorites(event: MouseEvent) {
    console.log("Add To Favorites");
    event.stopPropagation(); // Prevent modal from opening when clicking download
  
    if(this.generatedImage){
      this.imagesService.postAddToFavorites(this.generatedImage).subscribe(response =>{
        console.log(response);
        
      }, error => {
      console.error("Error Adding Image to Favorites:", error.error);
      this.showToast(error.error);
      this.isLoading = false;
    })
      console.log("Added To Favorites");
      this.showToast("Added To Favorites");

    }
    else
      this.showToast("Image isn't Available");
  }

  generateImage() {
    // this.imageUrl = null;
    this.generatedImage = null;
    // this.images = null;
    this.isLoading = true;

    if (!this.prompt || !this.brandName) {
      this.isLoading = false;
      return;
    }

    this.sdxlService.generateImage(this.brandName, this.brandStyle, this.prompt).subscribe(response => {
      this.isLoading = false;
      // this.images = response.images.map((img: any) => ({
      //   src: `data:${img.imageData.contentType};base64,${img.imageData.fileContents}`,
      //   type: img.imageData.contentType,
        
      // }));

      this.imagesData = response.images.map((img: any)=>({
        id: img.imageId,
        userId: img.userId,
        prompt: img.prompt,
        image: `data:${img.imageData.contentType};base64,${img.imageData.fileContents}`,
        isFavorite: img.isFavorite
      }));
      
      if(this.imagesData){
        console.log(this.imagesData.length);
        this.generatedImage = this.imagesData[0];
      }
      
    }, error => {
      console.error("Error generating image:", error.error);
      this.showToast(error.error);
      this.isLoading = false;
    });
  }


  showToast(msg:string) {
    Toastify({
      text: msg,
      duration: 3000, // Duration in milliseconds
      close: true, // Adds a close button
      gravity: "top", // `top` or `bottom`
      position: "right", // `left`, `center`, or `right`
      backgroundColor: "#dc3545"
    }).showToast();
  }
  /** Open Pop-up Modal */
  openModal() {
    this.isModalOpen = true;
  }

  /** Close Pop-up Modal */
  closeModal() {
    this.isModalOpen = false;
  }

  /** Download Image */
  downloadImage(event: Event) {
    event.stopPropagation(); // Prevent modal from opening when clicking download

    if (this.generatedImage?.image) {
      this.convertToBlob(this.generatedImage.image).then(blob => {
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = 'generated-image.png'; // Set file name
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
      }).catch(error => {
        console.error("Error downloading image:", error);
      });
    }
  }
  private convertToBlob(imageUrl: string): Promise<Blob> {
    return fetch(imageUrl, { mode: 'cors' }).then(response => response.blob());
  }
}