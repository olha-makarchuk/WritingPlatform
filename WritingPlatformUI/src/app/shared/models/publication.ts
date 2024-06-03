import { Author } from "./author";

export interface Publication {
  publicationId: number;
  publicationName: string;
  genreName: string;
  authorName: string;
  rating: number;
  datePublication: Date;
  titleKey: string;
  
  bookDescription: string;
  author: Author;
}


// sort-publication-query.ts
export interface SortPublicationQuery {
  genreIds: number[];
  startPage: number;
  endPage: number;
  yearPublication: number;
  sortByItemId: number;
}
/*
export enum SortByItem {
  Rating = 'Rating',
  DateAdding = 'DateAdding',
  NumberReviews = 'NumberReviews'
}*/

export interface SortByItem {
  itemName: string;
  fieldName: string;
  id: number;
}
