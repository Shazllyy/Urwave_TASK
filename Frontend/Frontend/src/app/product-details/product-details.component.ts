import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProductService } from '../services/product.service';  // Import the service
import { ProductResponseDTO } from '../interfaces/product.model.model';  // Assuming you have a model for ProductResponseDTO
import { CommonModule } from '@angular/common';  // Import CommonModule

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.css'],
  standalone: true,  // Marking this as a standalone component
  imports: [CommonModule]  // Import CommonModule for using directives like ngClass
})
export class ProductDetailsComponent implements OnInit {
  selectedProduct: ProductResponseDTO | null = null;  // To hold the selected product details
  loading: boolean = false;  // Flag for loading state
  errorMessage: string = '';  // For storing error messages

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService  // Inject the ProductService
  ) {}

  ngOnInit(): void {
    // Get the 'id' from the URL
    const productId = this.route.snapshot.paramMap.get('id');
    if (productId) {
      this.fetchProductDetails(productId);
    }
  }

  // Fetch the product details based on id
  fetchProductDetails(id: string): void {
    this.loading = true;
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.selectedProduct = product;  // Store the product details
        this.loading = false;  // Set loading to false after data is fetched
      },
      error: (error) => {
        this.errorMessage = 'Error fetching product details';
        this.loading = false;  // Set loading to false on error
      }
    });
  }
}
