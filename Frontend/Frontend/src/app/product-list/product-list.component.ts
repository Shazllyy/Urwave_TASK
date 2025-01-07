import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../services/product.service';
import { ProductResponseDTO } from '../interfaces/product.model.model';
import { DropdownModule } from 'primeng/dropdown';
import { CategoryService } from '../services/category.service';
import { CategoryResponseDTO } from '../interfaces/Category';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    FormsModule,
    DropdownModule
  ],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.css']
})
export class ProductListComponent implements OnInit {
  products: ProductResponseDTO[] = [];
  filteredProducts: ProductResponseDTO[] = [];
  totalRecords: number = 0;
  loading: boolean = false;

  filteredName: string = '';
  filteredPrice: string | null = null;
  filteredCategory: string | null = null;
  filteredStatus: string | null = null;

  tableFilters: any = {};
  first: number = 0;
  rows: number = 10;

  sortField: string = '';
  sortOrder: number = 1;
  categoryOptions: { label: string, value: string }[] = [];

  priceOptions = [
    { label: 'Below $50', value: 'below50' },
    { label: '$50 - $100', value: '50-100' },
    { label: '$100 - $200', value: '100-200' },
    { label: 'Above $200', value: 'above200' }
  ];

  statusOptions = [
    { label: 'Active', value: 'Active' },
    { label: 'Inactive', value: 'Inactive' }
  ];

  constructor(private productService: ProductService, private categoryService: CategoryService) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe((categories: CategoryResponseDTO[]) => {
      this.categoryOptions = categories.map(category => ({
        label: category.name,
        value: category.id
      }));
    });
  }

  loadProducts() {
    this.loading = true;
    this.productService.getProducts(
      this.first,
      this.rows,
      this.filteredCategory || '',
      this.filteredPrice || '',
      this.filteredStatus || '',
      this.sortField,
      this.sortOrder
    ).subscribe((response: ProductResponseDTO[]) => {
      this.products = response;
      this.totalRecords = this.products.length;
      this.filteredProducts = [...this.products];
      this.loading = false;
    });
  }

  applyFilters() {
    this.filteredProducts = this.products.filter(product => {
      let priceMatch = true;
      if (this.filteredPrice) {
        switch (this.filteredPrice) {
          case 'below50':
            priceMatch = product.price < 50;
            break;
          case '50-100':
            priceMatch = product.price >= 50 && product.price <= 100;
            break;
          case '100-200':
            priceMatch = product.price >= 100 && product.price <= 200;
            break;
          case 'above200':
            priceMatch = product.price > 200;
            break;
        }
      }

      return (
        (this.filteredName ? product.name.toLowerCase().includes(this.filteredName.toLowerCase()) : true) &&
        priceMatch &&
        (this.filteredCategory ? product.categoryId === this.filteredCategory : true) &&
        (this.filteredStatus ? product.status === this.filteredStatus : true)
      );
    });

    this.first = 0;
    this.customSort({ field: this.sortField, order: this.sortOrder });
  }

  onLazyLoad(event: any) {
    this.first = event.first;
    this.rows = event.rows;
    this.sortField = event.sortField;
    this.sortOrder = event.sortOrder;
    this.customSort({ field: this.sortField, order: this.sortOrder });
  }

  customSort(event: any) {
    const field = event.field as keyof ProductResponseDTO;
    const order = event.order;

    this.filteredProducts.sort((a, b) => {
      let value1 = a[field];
      let value2 = b[field];

      if (value1 == null && value2 != null) return -1 * order;
      if (value1 != null && value2 == null) return 1 * order;
      if (value1 == null && value2 == null) return 0;

      if (typeof value1 === 'string' && typeof value2 === 'string') {
        return value1.localeCompare(value2) * order;
      }

      if (typeof value1 === 'number' && typeof value2 === 'number') {
        return (value1 - value2) * order;
      }

      return 0;
    });
  }

  editProduct(product: ProductResponseDTO) {
    product.isEditing = true;
  }

  saveProduct(product: ProductResponseDTO) {
    this.productService.updateProduct(product.id, product).subscribe(() => {
      product.isEditing = false;
      this.loadProducts();
    });
  }

  cancelEdit(product: ProductResponseDTO) {
    product.isEditing = false;
    this.loadProducts();
  }
}
