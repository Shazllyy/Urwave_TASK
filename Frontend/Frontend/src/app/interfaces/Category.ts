import { ProductResponseDTO } from '../interfaces/product.model.model';
export interface CategoryRequestDTO {
    Name: string;
    Description: string;
    ParentCategoryId?: string | null;
    Status: number;
  }
  export interface CategoryResponseDTO {
    id: string;
    name: string;
    description: string;
    parentCategoryId?: string | null;
    status: string;
    createdDate: string;
    updatedDate: string;
    isEditing?: boolean;  
    subcategories?: CategoryResponseDTO[]; // Mapping to Subcategories list in C#
  products?: ProductResponseDTO[]; // Mapping to Products list in C#

  }
    