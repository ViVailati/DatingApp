import {Component, inject, OnInit, signal} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {NgOptimizedImage} from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [
    NgOptimizedImage
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private http = inject(HttpClient);

  protected readonly title = signal('Dating app');
  protected members = signal<any>([]);

  ngOnInit(): void {
    this.http.get("https://localhost:7087/api/members").subscribe({
      next: res => {
        this.members.set(res);
      },
      error: err => {
        console.error(err);
      }
    });
  }
}
