import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomeComponent } from './components/home/home.component';
import { authGuard } from './guards/auth.guard';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ProductListComponent } from './product-list/product-list.component';
import { CategoryListComponent } from './category-list/category-list.component';
import { EditCategoryComponent } from './category-edit/category-edit.component';
import { CategoryCreateComponent } from './category-create/category-create.component';
import { ProductCreateComponent } from './product-create/product-create.component';
import { ProductEditComponent } from './product-edit/product-edit.component';
import { ProductDetailsComponent } from './product-details/product-details.component';
import { CategoryDetailsComponent } from './category-details/category-details.component';
import { UploadComponent } from './upload/upload.component';


export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'home', component: HomeComponent, canActivate: [authGuard] },
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'p', component: ProductListComponent },
  { path: 'p/create', component: ProductCreateComponent },
  { path: 'product-edit/:id', component: ProductEditComponent },  // Route for editing a product


  { path: 'C', component: CategoryListComponent },
  { path: 'C/edit/:id', component: EditCategoryComponent }, // Route for editing a category
  { path: 'C/create', component: CategoryCreateComponent }, // Route for editing a category
  { path: 'product-details/:id', component: ProductDetailsComponent },  // Route for displaying product details
  { path: 'cdetails/:id', component: CategoryDetailsComponent },  // Route for displaying product details
  { path: 'u', component: UploadComponent },  // Route for displaying product details









];
