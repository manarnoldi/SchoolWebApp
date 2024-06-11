import { Injectable } from '@angular/core';
import { ResourceModel } from '../models/ResourceModel';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export abstract class ResourceService<T extends ResourceModel<T>> {
  constructor(
      private httpClient: HttpClient,
      private tConstructor: {new (m: Partial<T>, ...args: unknown[]): T}
  ) {}

  public create(
      url,
      resource: Partial<T> & {toJson: () => T}
  ): Observable<T> {
      return this.httpClient
          .post<T>(url, resource.toJson())
          .pipe(map((result) => new this.tConstructor(result)));
  }

  public get(url, params?: any): Observable<T[]> {
      let httpParams = new HttpParams();
      if (params) {
          Object.keys(params).forEach((key) => {
              httpParams = httpParams.append(
                  key,
                  JSON.stringify(params[key])
              );
          });
      }

      return this.httpClient
          .get<T[]>(`${url}`, {params: httpParams})
          .pipe(map((result) => result.map((i) => new this.tConstructor(i))));
  }

  public getById(id: number, url): Observable<T> {
      return this.httpClient
          .get<T>(`${url}/${id}`)
          .pipe(map((result) => new this.tConstructor(result)));
  }

  public getCount(url): Observable<number> {
      return this.httpClient.get<number>(`${url}`);
  }

  public getByIds(ids: number[], url): Observable<T> {
      return this.httpClient
          .get<T>(`${url}/${ids.join('/')}`)
          .pipe(map((result) => new this.tConstructor(result)));
  }

  public update(
      url,
      resource: Partial<T> & {toJson: () => T}
  ): Observable<T> {
      return this.httpClient
          .put<T>(`${url}/${resource.id}`, resource.toJson())
          .pipe(map((result) => new this.tConstructor(result)));
  }

  public updateByIds(
      url,
      resource: Partial<T> & {toJson: () => T},
      ids: number[]
  ): Observable<T> {
      return this.httpClient
          .put<T>(`${url}/${ids.join('/')}`, resource.toJson())
          .pipe(map((result) => new this.tConstructor(result)));
  }

  public delete(url, id: number): Observable<void> {
      return this.httpClient.delete<void>(`${url}/${id}`);
  }

  public deleteByIds(url, ids: number[]): Observable<void> {
      return this.httpClient.delete<void>(`${url}/${ids.join('/')}`);
  }
}
