import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../services/category.service';
import { CategoryRequestDTO, CategoryResponseDTO } from '../interfaces/Category';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-category-create',
  standalone: true,
  imports: [CommonModule, FormsModule, DropdownModule, ButtonModule],
  templateUrl: './category-create.component.html',
  styleUrls: ['./category-create.component.css']
})
export class CategoryCreateComponent implements OnInit {
  categoryForm: CategoryRequestDTO = {
    Name: '',
    Description: '',
    Status: 0,  // Default status is 0 for Active
    ParentCategoryId: null  // Optional parent category
  };
  categoryOptions: { label: string, value: string }[] = [];  // For parent category dropdown options
  statusOptions = [
    { label: 'Active', value: 0 },  
    { label: 'Inactive', value: 1 }  
  ];  // Status dropdown options
  isLoading: boolean = false;

  constructor(
    private categoryService: CategoryService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadParentCategoryOptions();  // Load available categories to use as parent options
  }

  // Fetch existing categories to use as potential parent categories
  loadParentCategoryOptions(): void {
    this.categoryService.getCategories().subscribe((categories: CategoryResponseDTO[]) => {
      this.categoryOptions = categories.map(category => ({
        label: category.name,  // Display name
        value: category.id     // Use ID for the parent category
      }));
      
    });
  }

  createCategory(): void {
    console.log('Form Data before submit:', this.categoryForm);  // Log categoryForm here
    if (this.categoryForm.Name) {
      this.isLoading = true;
  
      const newCategory: CategoryRequestDTO = {
        Name: this.categoryForm.Name,
        Description: this.categoryForm.Description,
        ParentCategoryId: this.categoryForm.ParentCategoryId,  // Ensure ParentCategoryId is passed
        Status: this.categoryForm.Status,
      };
  
      this.categoryService.createCategory(newCategory).subscribe(
        (response) => {
          this.isLoading = false;
          console.log('Category created successfully');
          this.router.navigate(['/C']);
        },
        (error) => {
          this.isLoading = false;
          console.error('Error creating category:', error);
        }
      );
    }
  }
  
}
