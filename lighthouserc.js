module.exports = {
  ci: {
    collect: {
      // Test the running Blazor Server app
      url: ['http://localhost:5000'],
      // Wait for app to be ready
      startServerCommand: 'dotnet run --configuration Release',
      startServerReadyPattern: 'Now listening on',
      startServerReadyTimeout: 30000,
      numberOfRuns: 1
    },
    assert: {
      // Performance budgets for Blazor Gantt components
      assertions: {
        'categories:performance': ['warn', { minScore: 0.8 }],
        'categories:accessibility': ['error', { minScore: 0.9 }],
        'categories:best-practices': ['warn', { minScore: 0.85 }],
        'categories:seo': ['warn', { minScore: 0.8 }]
      }
    },
    upload: {
      target: 'temporary-public-storage'
    }
  }
};
