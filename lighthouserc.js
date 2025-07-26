module.exports = {
  ci: {
    collect: {
      // Test the running Blazor Server app
      url: ['http://localhost:5000'],
      // Don't start server (we handle it manually)
      startServerCommand: null,
      numberOfRuns: 1
    },
    assert: {
      // Performance budgets for Blazor Gantt components
      assertions: {
        'categories:performance': ['warn', { minScore: 0.6 }],
        'categories:accessibility': ['error', { minScore: 0.9 }],
        'categories:best-practices': ['warn', { minScore: 0.8 }],
        'categories:seo': ['warn', { minScore: 0.7 }]
      }
    },
    upload: {
      target: 'temporary-public-storage'
    }
  }
};
