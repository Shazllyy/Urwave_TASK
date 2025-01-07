import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../services/category.service';
import { ProductResponseDTO } from '../interfaces/product.model.model';
import { CategoryResponseDTO } from '../interfaces/Category';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-category-details',
  standalone: true,
  imports: [CommonModule],  // Ensure CommonModule is included here
  templateUrl: './category-details.component.html',
  styleUrls: ['./category-details.component.css']
})
export class CategoryDetailsComponent implements OnInit {
  categoryId: string = '';
  category: CategoryResponseDTO | null = null;
  subcategories: CategoryResponseDTO[] = []; // Initialize as empty array
  products: ProductResponseDTO[] = []; // Initialize as empty array
  isLoading: boolean = true;
  errorMessage: string = '';

  constructor(
    private categoryService: CategoryService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadCategoryDetails();
  }

  loadCategoryDetails(): void {
    this.categoryId = this.route.snapshot.paramMap.get('id') || '';

    if (this.categoryId) {
      this.categoryService.getCategoryById(this.categoryId).subscribe(
        (category: CategoryResponseDTO) => {
          this.category = category;
          this.subcategories = category.subcategories || []; // If subcategories are undefined, fallback to empty array
          this.products = category.products || []; // Fallback to empty array for main category products
          console.log('Category details:', this.category);
        },
        (error) => {
          this.errorMessage = 'Error loading category details.';
          this.isLoading = false;
          console.error('Error loading category details:', error);
        }
      );
    } else {
      this.errorMessage = 'Category ID is missing.';
      this.isLoading = false;
    }
  }
}
