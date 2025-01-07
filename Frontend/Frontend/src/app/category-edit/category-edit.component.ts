import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { CategoryResponseDTO, CategoryRequestDTO } from '../interfaces/Category';
import { CategoryService } from '../services/category.service';

@Component({
  selector: 'app-edit-category',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonModule, InputTextModule, DropdownModule],
  templateUrl: './category-edit.component.html',
  styleUrls: ['./category-edit.component.css']
})
export class EditCategoryComponent implements OnInit {
  category: CategoryResponseDTO = {
    id: '',
    name: '',
    description: '',
    status: '',
    createdDate: '',
    updatedDate: '',
  };

  statusOptions = [
    { label: 'Active', value: 'Active' },
    { label: 'Inactive', value: 'Inactive' },
  ];

  constructor(
    private categoryService: CategoryService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Retrieve the category ID from the route parameters
    const categoryId = this.activatedRoute.snapshot.paramMap.get('id');
    
    if (categoryId) {
      this.loadCategory(categoryId);  // Load category data by ID
    } else {
      console.error('Category ID not found in URL');
    }
  }

  // Method to fetch category details by ID
  loadCategory(id: string) {
    this.categoryService.getCategoryById(id).subscribe(
      (data) => {
        this.category = data;  // Populate the form with category data
        console.log(data);
      },
      (error) => {
        console.error('Error loading category:', error);
      }
    );
  }

  onSubmit() {
    const updatedCategory: CategoryRequestDTO = {
      Name: this.category.name,  // Ensure 'Name' matches the backend property
      Description: this.category.description,  // Ensure 'Description' matches the backend property
      ParentCategoryId: this.category.parentCategoryId || null,  // Ensure ParentCategoryId is a valid GUID or null
      Status: this.category.status === 'Active' ? 0 : 1,  // Send Status as integer: 0 for Active, 1 for Inactive
    };
  
    this.categoryService.updateCategory(this.category.id, updatedCategory).subscribe({
      next: () => {
        console.log('Category updated successfully');
        this.router.navigate(['/C']);  // Navigate back to category list
      },
      error: (err) => {
        console.error('Error updating category:', err);
      },
    });
  }
  
  
  

  // Method to handle cancel action
  onCancel() {
    this.router.navigate(['/C']);  // Navigate back to category list if cancelled
  }
}
