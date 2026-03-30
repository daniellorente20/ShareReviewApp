import type { CreateProductRequest, Product } from '../types';

const BASE = '/api/products';

export async function getProducts(): Promise<Product[]> {
  const res = await fetch(BASE);
  if (!res.ok) throw new Error('Failed to fetch products');
  return res.json();
}

export async function getProduct(id: string): Promise<Product> {
  const res = await fetch(`${BASE}/${id}`);
  if (res.status === 404) throw new Error('Product not found');
  if (!res.ok) throw new Error('Failed to fetch product');
  return res.json();
}

export async function createProduct(data: CreateProductRequest): Promise<Product> {
  const res = await fetch(BASE, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error('Failed to create product');
  return res.json();
}
