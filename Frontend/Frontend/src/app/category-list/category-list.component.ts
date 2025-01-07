import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../services/category.service';
import { CategoryResponseDTO } from '../interfaces/Category';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { DropdownModule } from 'primeng/dropdown';
import { Router } from '@angular/router';

@Component({
  selector: 'app-category-list',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    FormsModule,
    DropdownModule,
  ],
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.css']
})
export class CategoryListComponent implements OnInit {
  isDeleteModalVisible: boolean = false;
  selectedCategoryId: string | undefined;
  categories: CategoryResponseDTO[] = [];
  filteredCategories: CategoryResponseDTO[] = [];
  totalRecords: number = 0;
  loading: boolean = false;

  filteredName: string = '';
  filteredStatus: string | null = null;
  categoryToDelete: CategoryResponseDTO | undefined;
  categoryOptions: { label: string, value: string }[] = [];

  tableFilters: any = {};
  first: number = 0;
  rows: number = 10;

  statusOptions = [
    { label: 'Active', value: 'Active' },
    { label: 'Inactive', value: 'Inactive' }
  ];

  constructor(private categoryService: CategoryService, private router: Router) {}

  ngOnInit(): void {
    this.loadCategories();  // Fetch categories on initialization
    this.getCategoryOptions();  // Fetch category options for the dropdown
  }

  // Method to load all categories
  loadCategories() {
    this.loading = true;
    this.categoryService.getCategories().subscribe((response: CategoryResponseDTO[]) => {
      this.categories = response;
      this.totalRecords = this.categories.length;
      this.filteredCategories = [...this.categories];
      this.loading = false;
    });
  }

  // Open delete modal and set category to be deleted
  openDeleteModal(category: CategoryResponseDTO) {
    this.categoryToDelete = category;
    this.isDeleteModalVisible = true;
  }

  // Apply filters to categories
  applyFilters() {
    this.filteredCategories = this.categories.filter(category => {
      return (
        (this.filteredName ? category.name.toLowerCase().includes(this.filteredName.toLowerCase()) : true) &&
        (this.filteredStatus ? category.status === this.filteredStatus : true)
      );
    });

    this.first = 0;
  }

  // Handle lazy loading for pagination
  onLazyLoad(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.loadCategories();  // Fetch categories based on pagination
  }

  // Navigate to edit category page
  editCategory(category: CategoryResponseDTO) {
    this.router.navigate([`/C/edit/${category.id}`]);  // Navigate to the edit page with the category ID
  }

  // Fetch category options for the delete modal dropdown
  getCategoryOptions(): void {
    this.categoryService.getCategories().subscribe((categories: CategoryResponseDTO[]) => {
      this.categoryOptions = categories.map(category => ({
        label: category.name,   // Display the category name
        value: category.id     // Pass the category ID when selected
      }));
    });
  }

  // Cancel the edit operation and reload categories
  cancelEdit(category: CategoryResponseDTO) {
    category.isEditing = false;
    this.loadCategories();
  }

  // Delete category and open modal
  deleteCategory(category: CategoryResponseDTO) {
    this.openDeleteModal(category);  // Open the modal for deletion
  }

  // Cancel delete operation and close the modal
  cancelDelete() {
    this.isDeleteModalVisible = false;
    this.selectedCategoryId = undefined;  // Clear selection
  }

  // Confirm the delete operation and send the request to delete the category
  confirmDelete() {
    if (this.categoryToDelete) {
      this.categoryService.deleteCategory(this.categoryToDelete.id, this.selectedCategoryId)
        .subscribe(() => {
          this.loadCategories();  // Reload the categories list
          this.cancelDelete();  // Close the modal
        });
    }
  }
}
