import { Author } from "./author";

export interface Publication {
  publicationId: number;
  publicationName: string;
  genreName: string;
  authorName: string;
  rating: number;
  datePublication: Date;
  titleKey: string;
  fileKey: string;
  countOfPages:number;
  countOfRewiews:number;
  bookDescription: string;
  author: Author;
  paginatorCount: number;
}

export interface SortPublicationQuery {
  genreIds: number[];
  startPage: number;
  endPage: number;
  yearPublication: number;
  sortByItemId: number;
}

export interface SortByItem {
  itemName: string;
  fieldName: string;
  id: number;
}
