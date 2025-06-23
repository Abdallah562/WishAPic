import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GenerateImageComponent } from './generate-image/generate-image.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';
import { HistoryComponent } from './history/history.component';
import { HomeComponent } from './home/home.component';
import { FavoritesComponent } from './favorites/favorites.component';

const routes: Routes = [
  { path: "login", component: LoginComponent },
  { path: 'generate-image', component: GenerateImageComponent }, // Route to your page
  { path: 'register', component: RegisterComponent },
  { path: 'app-history', component: HistoryComponent},
  { path: 'app-home', component: HomeComponent},
  { path: 'app-favorites', component: FavoritesComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
