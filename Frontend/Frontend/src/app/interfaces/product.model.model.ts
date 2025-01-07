export interface ProductResponseDTO {
  id: string;
  name: string;
  description: string;
  price: number;
  categoryId: string;
  stockQuantity: number;
  imageUrl: string;
  createdDate: string;
  updatedDate: string;
  isEditing?: boolean;  
  status: string;
  categoryName : string;

}

  
  export interface ProductRequestDTO {
    name: string;
    description: string;
    price: number;
    categoryId: string;
    stockQuantity: number;
    imageUrl: string;
    status: number;

  }
  