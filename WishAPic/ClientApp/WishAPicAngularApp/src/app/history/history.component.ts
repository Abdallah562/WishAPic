import { Component } from '@angular/core';
import { SdxlService } from '../services/sdxl.service';
import Toastify from 'toastify-js';
import { ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { ImagesService } from '../services/images.service';
import { ImgData } from '../models/image-data';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrl: './history.component.css'
})
export class HistoryComponent {
  userId: string | null = null;
  imageData: ImgData[] | null = null;
    constructor(private accountService: AccountService, private imagesService: ImagesService) {}
  
  ngOnInit() {
    this.userId = this.accountService.getUserId();
    console.log('Received ID:', this.userId);

    if(this.userId){
      this.imagesService.getHistory(this.userId).subscribe(response => {
      this.imageData = response.map((img: any)=>({
            id: img.imageId,
            userId: img.userId,
            prompt: img.prompt,
            image: `data:${img.image.contentType};base64,${img.image.fileContents}`,
            isFavorite: img.isFavorite
        }));

      }, error => {
        this.showToast(error.error.error);
      });  
    }
  }

  onRemoveFromFavoritesClicked(imageData: ImgData) {
    console.log(imageData.id);
    
    this.imagesService.deleteFromFavorites(imageData).subscribe(response =>{
      console.log(response);
    }, error =>{
      this.showToast(error.error.error);
    })
      this.showToast("Deleted from Favorites");
  }
  onAddClicked(imageData: ImgData) {
    this.imagesService.postAddToFavorites(imageData).subscribe(response =>{
      console.log(response);
    }, error =>{
      this.showToast(error.error.error);
    })
      this.showToast("Added To Favorites");
  }
  
  onRemoveFromHistoryClicked(imageData: ImgData) {
    console.log(imageData.id);
    
    this.imagesService.deleteFromHistory(imageData).subscribe(response =>{
      console.log(response);
    }, error =>{
      this.showToast(error.error.error);
    })
      this.showToast("Deleted from History");  
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

}
