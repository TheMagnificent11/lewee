#!/usr/bin/env python3
"""
Diff Coverage Check Script

Analyzes code coverage for changed files and enforces a minimum threshold.
"""

import argparse
import sys
import os
import xml.etree.ElementTree as ET


def normalize_path(path):
    """Normalize file paths for comparison"""
    return path.replace('\\', '/').lower()


def extract_filename(full_path):
    """Extract just the filename from a full path"""
    return os.path.basename(full_path)


def parse_changed_files(changed_files_str):
    """Parse the changed files string into a list"""
    if not changed_files_str or not changed_files_str.strip():
        return []
    return [f.strip() for f in changed_files_str.split('\n') if f.strip()]


def analyze_coverage(coverage_file, threshold, changed_files):
    """Analyze coverage for the changed files"""
    
    # Load coverage data from Cobertura XML
    try:
        tree = ET.parse(coverage_file)
        root = tree.getroot()
    except Exception as e:
        print(f"❌ Error reading coverage report: {e}")
        sys.exit(1)
    
    if not changed_files:
        print("✅ No changed files to check")
        return
        
    print(f"🔍 Checking coverage for {len(changed_files)} changed files...")
    
    failed_files = []
    checked_files = []
    
    # Parse Cobertura XML - find all classes
    packages = root.findall('.//package')
    classes = []
    for package in packages:
        classes.extend(package.findall('.//class'))
    
    if not classes:
        print("⚠️  No class coverage data found")
    
    for class_elem in classes:
        filename = class_elem.get('filename', '')
        classname = class_elem.get('name', '')
        line_rate = float(class_elem.get('line-rate', '0'))
        
        if not filename:
            continue
        
        # Check if this file matches any of our changed files
        filename_normalized = normalize_path(filename)
        
        for changed_file in changed_files:
            changed_file_normalized = normalize_path(changed_file)
            
            # Try multiple matching strategies
            matches = (
                changed_file_normalized in filename_normalized or
                filename_normalized.endswith(changed_file_normalized) or
                extract_filename(changed_file_normalized) == extract_filename(filename_normalized)
            )
            
            if matches:
                coverage_percentage = round(line_rate * 100, 2)
                
                # Get line counts from the XML
                lines = class_elem.findall('.//line')
                total_lines = len(lines)
                covered_lines = len([line for line in lines if line.get('hits', '0') != '0'])
                
                checked_files.append({
                    'file': changed_file,
                    'class': classname,
                    'coverage': coverage_percentage,
                    'covered_lines': covered_lines,
                    'total_lines': total_lines,
                    'filename': filename
                })
                
                status = "✅" if coverage_percentage >= threshold else "❌"
                print(f"{status} {changed_file}: {coverage_percentage}% ({covered_lines}/{total_lines} lines)")
                
                if coverage_percentage < threshold:
                    failed_files.append({
                        'file': changed_file,
                        'coverage': coverage_percentage,
                        'covered_lines': covered_lines,
                        'total_lines': total_lines
                    })
                break
    
    # Handle files that weren't found in coverage report
    found_file_names = [extract_filename(normalize_path(f['file'])) for f in checked_files]
    missing_files = []
    
    for changed_file in changed_files:
        if extract_filename(normalize_path(changed_file)) not in found_file_names:
            missing_files.append(changed_file)
    
    print(f"\n📊 Coverage Summary:")
    print(f"   • Changed files: {len(changed_files)}")
    print(f"   • Files with coverage data: {len(checked_files)}")
    print(f"   • Files without coverage data: {len(missing_files)}")
    
    if missing_files:
        print(f"\n⚠️  Files not found in coverage report:")
        for missing_file in missing_files:
            print(f"   - {missing_file}")
        print("   This could mean:")
        print("   - Files contain no coverable code (e.g., only interfaces, enums, attributes)")
        print("   - Files are not included in test coverage collection")
        print("   - Files are generated code or excluded from coverage")
    
    # Final result
    if failed_files:
        print(f"\n❌ DIFF COVERAGE CHECK FAILED")
        print(f"   {len(failed_files)} of {len(checked_files)} checked files are below {threshold}% coverage:")
        for file_info in failed_files:
            needed_lines = max(0, int((file_info['total_lines'] * threshold / 100) - file_info['covered_lines']))
            print(f"   - {file_info['file']}: {file_info['coverage']}% (need ~{needed_lines} more covered lines)")
        print(f"\n💡 Please add tests to increase coverage for the files listed above.")
        sys.exit(1)
    elif checked_files:
        print(f"\n✅ DIFF COVERAGE CHECK PASSED")
        print(f"   All {len(checked_files)} checked files meet the {threshold}% coverage threshold!")
        if missing_files:
            print(f"   Note: {len(missing_files)} files had no coverage data (likely non-coverable code)")
    else:
        print(f"\n⚠️  No coverage data found for any changed files")
        if missing_files and len(missing_files) == len(changed_files):
            # All files are missing - this might be OK if they're all interfaces/enums
            print("   All changed files appear to be non-coverable code (interfaces, enums, etc.)")
            print("✅ Coverage check passed - no coverable code changes detected")
        else:
            print("❌ Failing build - expected coverage data but found none")
            sys.exit(1)


def main():
    parser = argparse.ArgumentParser(description='Check diff coverage for changed files')
    parser.add_argument('--coverage-file', required=True, help='Path to Cobertura coverage XML file')
    parser.add_argument('--threshold', type=float, required=True, help='Minimum coverage threshold (0-100)')
    parser.add_argument('--changed-files', required=True, help='Newline-separated list of changed files')
    
    args = parser.parse_args()
    
    changed_files = parse_changed_files(args.changed_files)
    analyze_coverage(args.coverage_file, args.threshold, changed_files)


if __name__ == '__main__':
    main()