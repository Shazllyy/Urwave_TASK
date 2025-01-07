import { Component, OnInit } from '@angular/core';
import { DashboardService } from '../../services/dashboard.service';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { TableModule } from 'primeng/table';
import { HttpClientModule } from '@angular/common/http';  // Import HttpClientModule here
import { CommonModule } from '@angular/common';  // Import CommonModule for *ngIf

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  standalone: true,  // Declare as a standalone component
  imports: [         // Import the necessary PrimeNG and HttpClientModule modules
    CardModule,
    ChartModule,
    TableModule,
    HttpClientModule, // Import HttpClientModule to make HTTP requests within this component
    CommonModule      // Import CommonModule for *ngIf and other directives
  ]
})
export class DashboardComponent implements OnInit {
  totalProducts: number = 0;
  totalCategories: number = 0;
  productsPerCategory: any[] = [];
  lowStockProducts: any[] = [];
  recentActivities: any[] = [];

  // Chart data
  productsChartData: any;
  lowStockChartData: any;

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.dashboardService.getDashboardMetrics().subscribe(
      data => {
        // Parse the data correctly into the component's properties
        console.log(data);
        console.log('Total Categories:', this.totalCategories);
        console.log('Total Products:', this.totalProducts);
        this.totalProducts = data.TotalProducts || 0;
        this.totalCategories = data.TotalCategories || 0;
        
        // Assuming these are arrays now, we can map through them
        this.productsPerCategory = Array.isArray(data.ProductsPerCategory) ? data.ProductsPerCategory : [];
        this.lowStockProducts = Array.isArray(data.LowStockProducts) ? data.LowStockProducts : [];
        this.recentActivities = Array.isArray(data.RecentActivities) ? data.RecentActivities : [];

        // Prepare chart data for 'Products per Category'
        if (this.productsPerCategory.length > 0) {
          this.productsChartData = {
            labels: this.productsPerCategory.map(item => item.category),
            datasets: [{
              data: this.productsPerCategory.map(item => item.productCount),
              label: 'Products per Category',
              backgroundColor: '#42A5F5',
            }]
          };
        }

        // Prepare chart data for 'Low Stock Products'
        if (this.lowStockProducts.length > 0) {
          this.lowStockChartData = {
            labels: this.lowStockProducts.map(item => item.productName),
            datasets: [{
              data: this.lowStockProducts.map(item => item.stockQuantity),
              label: 'Low Stock Products',
              backgroundColor: '#FF7043',
            }]
          };
        }
      },
      error => {
        console.error('Error loading dashboard data', error);
      }
    );
  }
}
