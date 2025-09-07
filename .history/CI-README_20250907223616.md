# ShopFlow CI - Minimal

## Status

- ✅ Build: Success (.NET 9.0)
- ✅ Tests: 352/352 Pass
- ✅ CI: Active (GitHub Actions)

## Workflow

File: `.github/workflows/ci-basic.yml`

**Triggers:** Push to `main`/`Ray`, PR to `main`

**Steps:**

1. Checkout
2. Setup .NET 9.0.x
3. Restore
4. Build (Release)
5. Test

## Usage

```bash
git add .
git commit -m "changes"
git push
```

## Local Testing

```bash
cd ShopFlow
dotnet restore ShopFlow.sln
dotnet build ShopFlow.sln --configuration Release
dotnet test ShopFlow.sln --configuration Release
```

## Philosophy

**Keep It Simple** - Just build and test, nothing else.

---

Last Updated: September 7, 2025
