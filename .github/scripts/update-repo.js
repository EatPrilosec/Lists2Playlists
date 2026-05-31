#!/usr/bin/env node

const fs = require('fs');
const path = require('path');

const version = process.argv[2];
const downloadUrl = process.argv[3];

if (!version || !downloadUrl) {
  console.error('Usage: update-repo.js <version> <downloadUrl>');
  process.exit(1);
}

const repoPath = path.join(__dirname, '../../repository.json');
const pluginPath = path.join(__dirname, '../../plugin.json');
const pluginInfo = JSON.parse(fs.readFileSync(pluginPath, 'utf8'));

let repository = [];
if (fs.existsSync(repoPath)) {
  try {
    repository = JSON.parse(fs.readFileSync(repoPath, 'utf8'));
  } catch (e) {
    console.warn('Could not parse existing repository.json, starting fresh');
  }
}

// Find if this plugin already exists in the repository
const existingIndex = repository.findIndex(p => p.id === pluginInfo.id);

// Create the new entry
const newEntry = {
  ...pluginInfo,
  imageUrl: `https://raw.githubusercontent.com/${process.env.GITHUB_REPOSITORY}/main/icon.png`,
  sourceUrl: `https://github.com/${process.env.GITHUB_REPOSITORY}`,
  version: version,
  downloadUrl: downloadUrl,
  releaseDate: new Date().toISOString().split('T')[0],
  changelog: `Release v${version}. See https://github.com/${process.env.GITHUB_REPOSITORY}/releases/tag/v${version} for details.`
};

if (existingIndex >= 0) {
  // Update existing entry, but keep it at the top for the latest version
  repository[existingIndex] = newEntry;
  // Move to front
  const updated = repository.splice(existingIndex, 1)[0];
  repository.unshift(updated);
} else {
  // Add new entry at the beginning
  repository.unshift(newEntry);
}

// Write back to file
fs.writeFileSync(repoPath, JSON.stringify(repository, null, 2) + '\n');

console.log(`Updated repository.json with version ${version}`);