import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { ProductRequestDTO } from '../interfaces/product.model.model';
import { CategoryService } from '../services/category.service';
import { CategoryResponseDTO } from '../interfaces/Category';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-product-create',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule, ButtonModule, InputTextModule, InputNumberModule],
  templateUrl: './product-create.component.html',
  styleUrls: ['./product-create.component.css']
})
export class ProductCreateComponent implements OnInit {
  productForm: ProductRequestDTO = {
    name: '',
    description: '',
    price: 0,
    categoryId: '',  // Initialize as empty string
    stockQuantity: 0,
    imageUrl: '', // Store the image file name here
    status: 0,
  };

  categoryOptions: { label: string, value: string }[] = []; // For category dropdown options
  statusOptions = [
    { label: 'Active', value: 0 },
    { label: 'Inactive', value: 1 },
  ];
  isLoading: boolean = false;
  uploadedImageName: string | null = null; // Store the uploaded file name

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCategoryOptions();  // Load categories for dropdown
  }

  // Fetch available categories for product dropdown
  loadCategoryOptions(): void {
    this.categoryService.getCategories().subscribe((categories: CategoryResponseDTO[]) => {
      this.categoryOptions = categories.map(category => ({
        label: category.name,  // Display category name
        value: category.id     // Use category id as the value
      }));
    });
  }

  // Handle category selection change
  onCategoryChange(event: any): void {
    this.productForm.categoryId = event.value;  // Set the selected category id in the form
  }

  // Handle file upload
  onImageUpload(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.uploadedImageName = file.name; // Get file name and save it
      this.productForm.imageUrl = file.name; // Save file name to productForm
    }
  }

  // Submit form to create a new product
  createProduct(): void {
    if (this.productForm.name && this.productForm.categoryId && this.uploadedImageName) {
      this.isLoading = true;
  
      // Create FormData instance to handle file and product data
      const formData = new FormData();
      
      // Append the product data
      formData.append('Name', this.productForm.name);
      formData.append('Description', this.productForm.description);
      formData.append('Price', this.productForm.price.toString());
      formData.append('CategoryId', this.productForm.categoryId);
      formData.append('StockQuantity', this.productForm.stockQuantity.toString());
      formData.append('Status', this.productForm.status.toString());
  
      // Append the image file (assuming the image is already selected and uploaded)
      const imageFile = (document.getElementById('imageFile') as HTMLInputElement).files![0];
      formData.append('ImageFile', imageFile, imageFile.name);
      
  
      // Call the ProductService to create the product with image
      this.productService.createProductWithImage(formData).subscribe(
        (response) => {
          this.isLoading = false;
          this.router.navigate(['/products']);  // Navigate to products list after successful creation
        },
        (error) => {
          this.isLoading = false;
          console.error('Error creating product:', error);
        }
      );
    } else {
      console.error('Category or Image is not selected!');
    }
  }
  

}

