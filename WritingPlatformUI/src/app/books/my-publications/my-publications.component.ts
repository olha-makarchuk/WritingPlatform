import { Component, OnInit } from '@angular/core';
import { Publication } from '../../shared/models/publication';
import { ActivatedRoute } from '@angular/router';
import { UserService } from '../../_services/user.service';
import { TokenStorageService } from '../../_services/token-storage.service';

@Component({
  selector: 'app-my-publications',
  templateUrl: './my-publications.component.html',
  styleUrls: ['./my-publications.component.css']
})
export class MyPublicationsComponent implements OnInit {
  publications: Array<Publication> = [];
  isLoggedIn = false;
  userId: string = "";
  currentPage: number = 1;
  pageSize: number = 3; 
  totalPages: number = 0;

  constructor(
    private tokenStorageService: TokenStorageService,
    private route: ActivatedRoute,
    private userService: UserService
  ) { }
  
  ngOnInit(): void {
    this.isLoggedIn = !!this.tokenStorageService.getToken();
    
    const user = this.tokenStorageService.getUser();
    if (user) {
      this.userId = user.userId || null;
    }

    this.userService.getPublicationsByAuthor(this.userId).subscribe(publications => {
      this.publications = publications;
      this.totalPages = Math.ceil(publications.length / this.pageSize);
    });
  }

  onDeletePublication(publicationId: number): void {
    this.userService.deletePublication(publicationId).subscribe(() => {
      this.publications = this.publications.filter(publication => publication.publicationId !== publicationId);
      this.totalPages = Math.ceil(this.publications.length / this.pageSize);
    });
  }

  onPageChange(pageNumber: number): void {
    this.currentPage = pageNumber;
  }

  getPaginatedPublications(): Publication[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = Math.min(startIndex + this.pageSize, this.publications.length);
    return this.publications.slice(startIndex, endIndex);
  }
}
