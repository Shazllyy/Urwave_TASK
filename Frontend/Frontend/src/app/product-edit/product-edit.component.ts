import { Component, OnInit } from '@angular/core';
import { ProductService } from '../services/product.service';
import { ProductRequestDTO, ProductResponseDTO } from '../interfaces/product.model.model';
import { CategoryService } from '../services/category.service';
import { CategoryResponseDTO } from '../interfaces/Category';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';

@Component({
  selector: 'app-product-edit',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule, ButtonModule, InputTextModule, InputNumberModule],
  templateUrl: './product-edit.component.html',
  styleUrls: ['./product-edit.component.css']
})
export class ProductEditComponent implements OnInit {
  productForm: ProductRequestDTO = {
    name: '',
    description: '',
    price: 0,
    categoryId: '',  // Initialize as empty string
    stockQuantity: 0,
    imageUrl: '',
    status: 0,
  };

  categoryOptions: { label: string, value: string }[] = []; // For category dropdown options
  statusOptions = [
    { label: 'Active', value: 0 },
    { label: 'Inactive', value: 1 },
  ];
  isLoading: boolean = false;
  productId: string = '';

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router,
    private route: ActivatedRoute // To get product ID from the URL
  ) {}

  ngOnInit(): void {
    this.loadCategoryOptions();  // Load categories for dropdown
    this.loadProductData();  // Load the product data based on the ID from the URL
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

  // Load the product data based on the ID from the URL
  loadProductData(): void {
    this.productId = this.route.snapshot.paramMap.get('id') || '';  // Get product ID from URL params

    if (this.productId) {
      this.productService.getProductById(this.productId).subscribe(
        (product: ProductResponseDTO) => {
          // Pre-fill the form with the product data
          this.productForm = {
            name: product.name,
            description: product.description,
            price: product.price,
            categoryId: product.categoryId,
            stockQuantity: product.stockQuantity,
            imageUrl: product.imageUrl,
            status: product.status === '0' ? 0 : 1,  // Convert status to number (0: Active, 1: Inactive)
          };
        },
        (error) => {
          console.error('Error loading product data:', error);
        }
      );
    }
  }

  // Handle category selection change
  onCategoryChange(event: any): void {
    this.productForm.categoryId = event.value;  // Set the selected category ID in the form
  }

  // Submit the form to update the product
  updateProduct(): void {
    if (this.productForm.name && this.productForm.categoryId) {
      this.isLoading = true;

      const updatedProduct: ProductRequestDTO = {
        name: this.productForm.name,
        description: this.productForm.description,
        price: this.productForm.price,
        categoryId: this.productForm.categoryId,
        stockQuantity: this.productForm.stockQuantity,
        imageUrl: this.productForm.imageUrl,
        status: this.productForm.status,
      };

      // Call the ProductService to update the product
      this.productService.updateProduct(this.productId, updatedProduct).subscribe(
        (response) => {
          this.isLoading = false;
          console.log('Product updated successfully');
          this.router.navigate(['/products']);  // Navigate to products list after successful update
        },
        (error) => {
          this.isLoading = false;
          console.error('Error updating product:', error);
        }
      );
    } else {
      console.error('Please fill out the required fields!');
    }
  }
}
